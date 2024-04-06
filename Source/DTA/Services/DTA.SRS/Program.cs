using DTA.Extensions.Common;
using DTA.Extensions.Telemetry;
using DTA.Models;
using DTA.SRS.Extensions;
using DTA.SRS.Services;
using DTA.SRS.Services.Interfaces;

#if DEBUG_JIT
using DTA.Extensions.Swagger;
#endif

#if AOT
var builder = WebApplication.CreateSlimBuilder(args);
#elif JIT
var builder = WebApplication.CreateBuilder(args);
#endif

builder.WithServiceNames(out var serviceName, out var meterName);
builder.Configuration.AddEnvironmentVariables(prefix: serviceName);

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

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

builder.Services.AddHealthChecks();

#if JIT
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endif

var settings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;

logger.LogInformation("OpenTelemetry settings {@Settings}", settings);

var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

logger.LogInformation("Service name: {ServiceName}, version: {ServiceVersion}", serviceName, serviceVersion);

#if AOT
builder.Services.RegisterContextSerializers();
#endif

// Add OpenTelemetry ...
builder.SetupOpenTelemetry(options =>
{
    options.OpenTelemetrySettings = settings;
    options.ServiceName = serviceName;
    options.ServiceVersion = serviceVersion;
    options.MeterNames = [meterName];
});

var app = builder.Build();

app.InitializeMetrics(meterName, serviceVersion);

#if DEBUG_JIT
app.SetupSwagger();
//app.MapControllers();
#endif

app.MapDefaultEndpoints();
app.MapMinimalApi();

app.Run();
