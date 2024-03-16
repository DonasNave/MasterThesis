using System.Diagnostics.Metrics;
using FUS.Data;
using FUS.Services;
using FUS.Services.Interfaces;
using DTA.JIT.Extensions;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using DTA.Shared.Models;

var builder = WebApplication.CreateBuilder(args);
// File Upload Service

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.
builder.Configuration.AddEnvironmentVariables(prefix: "DAT_JIT_FUS_");

builder.Services.AddDbContext<FileContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IFileService, FileService>();

var settings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;
const string serviceName = "DAT-JIT-FUS";
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

// Add OpenTelemetry ...
builder.Services
    .AddOpenTelemetry()
    // ... Resource ...
    .ConfigureResource(resourceBuilder =>
    {
        resourceBuilder.AddService(
            serviceName: serviceName,
            serviceVersion: serviceVersion,
            serviceInstanceId: Environment.MachineName);
    })
    // ... Tracing ...
    .SetupMyOtelTracing(settings)
    // ... Metrics 
    .SetupMyOtelMetrics(settings);

// Add OpenTelemetry Logging
builder.Logging.SetupMyOtelLogging(settings, serviceName, serviceVersion);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var meter = new Meter("DAT-JIT-FUS-Meter", "1.0.0");
var counterUploads = meter.CreateCounter<long>("download_api_calls_counter");
var counterDownloads = meter.CreateCounter<long>("upload_api_calls_counter");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Swagger redirect
app.MapGet("/", context => 
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});

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
