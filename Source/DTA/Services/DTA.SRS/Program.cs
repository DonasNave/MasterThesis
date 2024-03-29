using System.Diagnostics.Metrics;
using DTA.Extensions;
using DTA.Models;
using SRS.Services;
using SRS.Services.Interfaces;

#if AOT
var builder = WebApplication.CreateSlimBuilder(args);
#else
var builder = WebApplication.CreateBuilder(args);
#endif

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

#if AOT
const string prefix = "DTA_AOT_SRS_";
const string serviceName = "DTA-AOT-SRS";
const string meterName = "DTA-AOT-SRS-Meter";
#else
const string prefix = "DTA_JIT_SRS_";
const string serviceName = "DTA-JIT-SRS";
const string meterName = "DTA-JIT-SRS-Meter";
#endif

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var settings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;

logger.LogInformation("OpenTelemetry settings {@Settings}", settings);

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
    
    // Swagger redirect
    app.MapGet("/", context => 
    {
        context.Response.Redirect("/swagger/index.html");
        return Task.CompletedTask;
    });
}

app.MapControllers();

app.MapDefaultEndpoints();

app.MapGet("/api/signals/{count:int}", async (int count, IReadingService readingService)  =>
{
    var signals = await readingService.GetRandomSignals(count);
    counterSignals.Add(1);
    logger.LogInformation("Signal API called {Count} signals", count);
    return signals;
});

app.Run();
