using System.Diagnostics.Metrics;
using FUS.Data;
using FUS.Services;
using FUS.Services.Interfaces;
using DTA.JIT.Extensions;
using Microsoft.EntityFrameworkCore;
using DTA.Shared.Models;

var builder = WebApplication.CreateBuilder(args);
// File Upload Service

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.
builder.Configuration.AddEnvironmentVariables(prefix: "DAT_JIT_FUS_");

builder.Services.AddHealthChecks();

builder.Services.AddDbContext<FileContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IFileService, FileService>();

var settings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;

const string serviceName = "DAT-JIT-FUS";
const string meterName = "DAT-JIT-FUS-Meter";
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

// Add OpenTelemetry ...
builder.SetupOpenTelemetry(options =>
{
    options.OpenTelemetrySettings = settings;
    options.ServiceName = serviceName;
    options.ServiceVersion = serviceVersion;
    options.MeterNames = [meterName];
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var meter = new Meter(meterName, serviceVersion);
var counterUploads = meter.CreateCounter<long>("download_api_calls_counter");
var counterDownloads = meter.CreateCounter<long>("upload_api_calls_counter");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Swagger redirect
app.MapGet("/", context => 
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});

app.MapDefaultEndpoints();

app.MapPost("/api/file/upload", async (IFormFile file, IFileService fileService) =>
{
    counterUploads.Add(1);
    return await fileService.Upload(file);
});

app.MapGet("/api/file/download/{id:int}", async (int id, IFileService fileService) =>
{
    counterDownloads.Add(1);
    return await fileService.Download(id);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
