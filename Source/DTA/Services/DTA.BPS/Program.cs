using DTA.BPS.Api.Rabbit;
using DTA.BPS.Extensions;
using DTA.Extensions.Common;
using DTA.Extensions.Telemetry;
using DTA.Models.Options;

#if DEBUG_JIT
using DTA.Extensions.Swagger;
#endif

// Create builder
#if AOT
var builder = WebApplication.CreateSlimBuilder(args);
#else
var builder = WebApplication.CreateBuilder(args);
#endif

// Set service names
builder.WithServiceNames(out var serviceName, out var meterName);
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

// Add Environment variables
builder.Configuration.AddEnvironmentVariables(prefix: $"{serviceName}_");

// Get application settings
var telemetrySettings = builder.Configuration.GetSection(nameof(OpenTelemetrySettings))
                                    .Get<OpenTelemetrySettings>()!;

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<ServiceConnectionOptions>(builder.Configuration.GetSection("ServiceConnections"));

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add OpenTelemetry ...
builder.SetupOpenTelemetry(options =>
{
    options.OpenTelemetrySettings = telemetrySettings;
    options.ServiceName = serviceName;
    options.ServiceVersion = serviceVersion;
    options.MeterNames = [meterName];
});

// Add services to the container.
builder.Services.AddHealthChecks();
builder.Services.RegisterServices();

#if DEBUG_JIT
builder.Services.AddSwaggerEndpoints();
#endif

var app = builder.Build();

#if DEBUG_JIT
app.SetupSwagger();
#endif

_ = app.Services.GetRequiredService<RabbitMqConsumerService>();

// Initialize metrics
app.InitializeMetrics(meterName, serviceVersion);

// Map endpoints
app.MapDefaultEndpoints();

app.Run();