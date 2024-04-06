using DTA.Extensions.Common;
using DTA.Extensions.Telemetry;
using DTA.SRS.Extensions;

#if AOT
using DTA.Models.Extensions;
using DTA.Models.JsonSerializers;
using DTA.SRS.Api.Rest;

#elif DEBUG_JIT
using DTA.Extensions.Swagger;
#endif

// Create builder
#if AOT
var builder = WebApplication.CreateSlimBuilder(args);
#elif JIT
var builder = WebApplication.CreateBuilder(args);
#endif

// Set service names
builder.WithServiceNames(out var serviceName, out var meterName);
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

builder.Configuration.AddEnvironmentVariables(prefix: serviceName);

// Service settings
var telemetrySettings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;

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
