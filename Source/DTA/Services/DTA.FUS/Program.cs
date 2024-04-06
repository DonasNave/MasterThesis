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

// Set service names

builder.WithServiceNames(out var serviceName, out var meterName);
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

builder.Configuration.AddEnvironmentVariables(prefix: serviceName);

// Get application settings
var telemetrySettings = builder.Configuration.GetSection(nameof(OpenTelemetrySettings))
                                             .Get<OpenTelemetrySettings>()!;

var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                         ?? throw new InvalidOperationException("Database connection string is missing");

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
