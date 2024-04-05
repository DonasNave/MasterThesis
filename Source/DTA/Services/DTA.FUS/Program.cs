using System.Diagnostics.Metrics;
using DTA.Extensions;
using DTA.FUS.Data;
using DTA.FUS.Migrations;
using DTA.FUS.Services;
using DTA.FUS.Services.gRPC;
using DTA.FUS.Services.Interfaces;
using DTA.Migrator;
using DTA.Models;

#if AOT
using Dapper;
using DTA.Models.JsonSerializers;

// Generally, the AOT module of Dapper is disabled, in order for it to work as expected, it's enabled in crucial segments via the DapperAot attribute
[module: DapperAot(false)]
#endif

#if AOT
var builder = WebApplication.CreateSlimBuilder(args);
#elif JIT
var builder = WebApplication.CreateBuilder(args);
#endif

builder.Services.AddGrpc();

#if AOT
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, CommonResponseContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(1, DtaFileContext.Default);
});
#endif

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.
#if AOT
const string compilationMode = "AOT";
#elif JIT
const string compilationMode = "JIT";
#endif

const string prefix = $"DTA_{compilationMode}_FUS_";
const string serviceName = $"DTA-{compilationMode}-FUS";
const string meterName = $"DTA-{compilationMode}-FUS-Meter";

builder.Configuration.AddEnvironmentVariables(prefix: prefix);

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

#if JIT
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endif

var meter = new Meter(meterName, serviceVersion);
var counterUploads = meter.CreateCounter<long>("download_api_calls_counter");
var counterDownloads = meter.CreateCounter<long>("upload_api_calls_counter");

var app = builder.Build();

app.MapGrpcService<GrpcFileService>();

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
}).DisableAntiforgery();

app.MapGet("/api/file/download/{id:int}", async (int id, IFileService fileService) =>
{
    counterDownloads.Add(1);
    return await fileService.Download(id);
});

app.Run();
