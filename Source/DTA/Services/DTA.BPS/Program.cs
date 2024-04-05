using System.Diagnostics.Metrics;
using DTA.BPS.Configuration;
using DTA.Extensions;
using DTA.Models;

#if AOT
var builder = WebApplication.CreateSlimBuilder(args);
#else
var builder = WebApplication.CreateBuilder(args);
#endif

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

#if AOT
const string prefix = "DTA_AOT_BPS_";
const string serviceName = "DTA-AOT-BPS";
const string meterName = "DTA-AOT-BPS-Meter";
#else
const string prefix = "DTA_JIT_BPS_";
const string serviceName = "DTA-JIT-BPS";
const string meterName = "DTA-JIT-BPS-Meter";
#endif

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Environment variables
builder.Configuration.AddEnvironmentVariables(prefix: prefix);

var fusUrl = builder.Configuration["ServiceConnections:FUS:Url"];

if (string.IsNullOrWhiteSpace(fusUrl))
{
    throw new InvalidOperationException("File service URL is missing");
}

ServiceConfiguration.FileServiceAddress = fusUrl;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

var meter = new Meter(meterName, serviceVersion);
var batchCounter = meter.CreateCounter<long>("batch_process_counter");
app.UseHttpsRedirection();

app.MapGet("/api/batch/process", () =>
    {
        batchCounter.Add(1);
        return Results.Ok("Batch process completed");
    })
    .WithName("BatchProcessing")
    .WithOpenApi();

app.Run();