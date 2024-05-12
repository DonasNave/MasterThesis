using DTA.Extensions.Common;
using DTA.Extensions.Telemetry;
using DTA.SRS.Api.Rest;
using DTA.SRS.Extensions;

#if AOT
using DTA.Models.Extensions;
using DTA.Models.JsonSerializers;

#elif DEBUG_JIT
using DTA.Extensions.Swagger;
#endif

// Create builder
#if AOT
var builder = WebApplication.CreateSlimBuilder(args);
#elif JIT
var builder = WebApplication.CreateBuilder(args);
#endif

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Set service names
builder.WithServiceNames(out var serviceName, out var meterName);
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

// Add Environment variables
builder.Configuration.AddEnvironmentVariables(prefix: $"{serviceName}_");

// Telemetry settings
#if AOT
var telemetrySettings = new OpenTelemetrySettings()
{
    ExporterEndpoint = new Uri(builder.Configuration["OpenTelemetrySettings:ExporterEndpoint"] ?? string.Empty),
    ExporterProtocol = builder.Configuration["OpenTelemetrySettings:ExporterProtocol"] ?? "grpc",
    ServiceNameSuffix = builder.Configuration["OpenTelemetrySettings:ServiceNameSuffix"]
};
#elif JIT
var telemetrySettings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;
#endif

// Append service name suffix
if (!string.IsNullOrEmpty(telemetrySettings.ServiceNameSuffix))
{
    serviceName += telemetrySettings.ServiceNameSuffix;
}

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
//builder.Services.AddControllers();
builder.Services.AddSwaggerEndpoints();
#endif

// Register context serializers in AOT mode
#if AOT
builder.Services.RegisterContextSerializers([ DtaSignalContext.Default ]);
#endif

// Build the app
var app = builder.Build();

// Log the service settings
var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Starting application");
logger.LogInformation("Service Name: {ServiceName}", serviceName);
logger.LogInformation("Service Version: {ServiceVersion}", serviceVersion);
logger.LogInformation("Telemetry Endpoint: {TelemetrySettingsExporterEndpoint}", telemetrySettings.ExporterEndpoint);
logger.LogInformation("Telemetry Endpoint Protocol: {TelemetrySettingsExporterProtocol}", telemetrySettings.ExporterProtocol);

// Initialize metrics
app.InitializeMetrics(meterName, serviceVersion);

// Map endpoints
app.MapDefaultEndpoints();
app.MapSignalModule();

#if DEBUG_JIT
app.SetupSwagger();
//app.MapControllers();
#endif

app.Run();
