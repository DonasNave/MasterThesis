using DTA.Extensions;
using DTA.Extensions.Common;
using DTA.Extensions.Telemetry;
using DTA.FUS.Api.Rest;
using DTA.FUS.Data;
using DTA.FUS.Extensions;
using DTA.FUS.Migrations;
using DTA.Migrator;

#if AOT
using Dapper;
using DTA.Models.Extensions;
using DTA.Models.JsonSerializers;

// Generally, the AOT module of Dapper is disabled, in order for it to work as expected, it's enabled in crucial segments via the DapperAot attribute
[module: DapperAot(false)]
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
    ExporterProtocol = builder.Configuration["OpenTelemetrySettings:ExporterProtocol"] ?? "grpc"
};
#elif JIT
var telemetrySettings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;
#endif

var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                         ?? throw new InvalidOperationException("Database connection string is missing");

// Add OpenTelemetry ...
builder.SetupOpenTelemetry(options =>
{
    options.OpenTelemetrySettings = telemetrySettings;
    options.ServiceName = serviceName;
    options.ServiceVersion = serviceVersion;
    options.MeterNames = [meterName];
});

// Ensure database exists and apply migrations
builder.Services
    .EnsureDatabaseExists(dbConnectionString)
    .InitiateMigrator(dbConnectionString)
    .ApplyMigrations(options =>
    {
        options.ConnectionString = dbConnectionString;
        options.Migrations.Add(new AddFileTable(1));
    });

DbConfiguration.DefaultConnectionString = dbConnectionString;

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddHealthChecks();
builder.Services.RegisterServices();

// Register context serializers in AOT mode
#if AOT
builder.Services.RegisterContextSerializers([ CommonResponseContext.Default, DtaFileContext.Default ]);
#endif

#if DEBUG_JIT
builder.Services.AddSwaggerEndpoints();
//builder.Services.AddControllers();
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
app.MapFileModule();

// Map gRPC services
app.MapGrpcServices();

#if DEBUG_JIT
app.SetupSwagger();
//app.MapControllers();
#endif

app.Run();
