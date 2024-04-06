using DTA.Extensions;
using DTA.Extensions.Common;
using DTA.Extensions.Telemetry;
using DTA.FUS.Data;
using DTA.FUS.Extensions;
using DTA.FUS.Migrations;
using DTA.FUS.Services;
using DTA.FUS.Services.Interfaces;
using DTA.Migrator;
using DTA.Models;

#if AOT
using Dapper;
// Generally, the AOT module of Dapper is disabled, in order for it to work as expected, it's enabled in crucial segments via the DapperAot attribute
[module: DapperAot(false)]
#endif

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

#if AOT
builder.Services.RegisterContextSerializers();
#endif

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddGrpc();
builder.Services.AddHealthChecks();

var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(dbConnectionString))
{
    throw new InvalidOperationException("Database connection string is missing");
}

DbConfiguration.DefaultConnectionString = dbConnectionString;

// Ensure database exists and apply migrations
builder.Services
    .EnsureDatabaseExists(dbConnectionString)
    .InitiateMigrator(dbConnectionString)
    .ApplyMigrations(options =>
    {
        options.ConnectionString = dbConnectionString;
        options.Migrations.Add(new AddFileTable(1));
    });

builder.Services.AddTransient<IFileService, FileService>();

var settings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;

var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

// Add OpenTelemetry ...
builder.SetupOpenTelemetry(options =>
{
    options.OpenTelemetrySettings = settings;
    options.ServiceName = serviceName;
    options.ServiceVersion = serviceVersion;
    options.MeterNames = [meterName];
});

#if DEBUG_JIT
builder.Services.AddSwaggerEndpoints();
//builder.Services.AddControllers();
#endif

var app = builder.Build();

app.InitializeMetrics(meterName, serviceVersion);
app.MapDefaultEndpoints();
app.MapMinimalApi();
app.MapGrpcServices();

#if DEBUG_JIT
app.SetupSwagger();
//app.MapControllers();
#endif

app.Run();
