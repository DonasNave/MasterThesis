using System.Diagnostics.Metrics;
using DTA.JIT.Extensions;
using DTA.Shared.Models;
using SRS.Services;
using SRS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
// Signal Readings Service

// Setup logging to console
var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Debug); // Adjust the log level as needed
});

ILogger logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Logging from the configuration phase");


// Add services to the container.
builder.Services.AddTransient<IReadingService, ReadingService>();

builder.Configuration.AddEnvironmentVariables(prefix: "DTA_JIT_SRS_");

builder.Services.AddHealthChecks();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var settings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;

logger.LogInformation("OpenTelemetry settings {@Settings}", settings);

const string serviceName = "DTA-JIT-SRS";
const string meterName = "DTA-JIT-SRS-Meter";
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

logger.LogInformation("Service name: {ServiceName}, version: {ServiceVersion}", serviceName, serviceVersion);

// Add OpenTelemetry ...
builder.SetupOpenTelemetry(options =>
{
    options.OpenTelemetrySettings = settings;
    options.ServiceName = serviceName;
    options.ServiceVersion = serviceVersion;
    options.MeterNames = [meterName];
});

var meter = new Meter(meterName, serviceVersion);
var counterSignals = meter.CreateCounter<long>("signal_api_calls_counter");

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

app.MapGet("/api/signals/{count:int}", async (int count, IReadingService readingService)  =>
{
    var signals = await readingService.GetRandomSignals(count);
    counterSignals.Add(1);
    logger.LogInformation("Signal API called {Count} signals", count);
    return signals;
});

app.Run();
