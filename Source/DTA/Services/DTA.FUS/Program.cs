using System.Diagnostics.Metrics;
using DTA.Extensions;
using FUS.Data;
using FUS.Services;
using FUS.Services.Interfaces;
using DTA.Models;
using Microsoft.EntityFrameworkCore;

#if AOT
var builder = WebApplication.CreateSlimBuilder(args);
#elif JIT
var builder = WebApplication.CreateBuilder(args);
#endif

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.
#if AOT
const string prefix = "DTA_AOT_FUS_";
const string serviceName = "DTA-AOT-FUS";
const string meterName = "DTA-AOT-FUS-Meter";
#else
const string prefix = "DTA_JIT_FUS_";
const string serviceName = "DTA-JIT-FUS";
const string meterName = "DTA-JIT-FUS-Meter";
#endif

builder.Configuration.AddEnvironmentVariables(prefix: prefix);

builder.Services.AddHealthChecks();

builder.Services.AddDbContext<FileContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IFileService, FileService>();

var settings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;

#if AOT
const string serviceVersion = "1.0.0";
#elif JIT
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";
#endif

// Add OpenTelemetry ...
builder.SetupOpenTelemetry(options =>
{
    options.OpenTelemetrySettings = settings;
    options.ServiceName = serviceName;
    options.ServiceVersion = serviceVersion;
    options.MeterNames = [meterName];
});

#if JIT
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endif

var meter = new Meter(meterName, serviceVersion);
var counterUploads = meter.CreateCounter<long>("download_api_calls_counter");
var counterDownloads = meter.CreateCounter<long>("upload_api_calls_counter");

var app = builder.Build();

#if JIT
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Swagger redirect
    app.MapGet("/", context => 
    {
        context.Response.Redirect("/swagger/index.html");
        return Task.CompletedTask;
    });
}

app.MapControllers();
#endif

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

app.Run();
