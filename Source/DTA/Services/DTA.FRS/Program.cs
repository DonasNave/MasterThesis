using System.Diagnostics.Metrics;
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
const string prefix = "DTA_AOT_FRS_";
const string serviceName = "DTA-AOT-FRS";
const string meterName = "DTA-AOT-FRS-Meter";
#else
const string prefix = "DTA_JIT_FRS_";
const string serviceName = "DTA-JIT-FRS";
const string meterName = "DTA-JIT-FRS-Meter";
#endif

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Environment variables
builder.Configuration.AddEnvironmentVariables(prefix: prefix);

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
var responseCounter = meter.CreateCounter<long>("response_counter");
app.UseHttpsRedirection();

app.MapGet("/api/response/status", () =>
    {
        responseCounter.Add(1);
        return (Random.Shared.Next() % 3) switch
        {
            0 => Results.Ok("Success"),
            1 => Results.NotFound("Not found"),
            _ => Results.Conflict("Data conflict")
        };
    })
    .WithName("StatusResponse")
    .WithOpenApi();

app.Run();