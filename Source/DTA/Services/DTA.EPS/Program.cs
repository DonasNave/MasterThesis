using System.Diagnostics.Metrics;
using DTA.Extensions.Common;
using DTA.Extensions.Telemetry;
using DTA.Models;
using RabbitMQ.Client;

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
var eventSimulatedCounter = meter.CreateCounter<long>("event_simulated_counter");

#if DEBUG_JIT
builder.Services.AddSwaggerEndpoints();
#endif

var app = builder.Build();

#if DEBUG_JIT
app.SetupSwagger();
#endif

app.MapGet("/api/simulateEvent/{id:int}", (int id) =>
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "simulated",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = BitConverter.GetBytes(id);

        channel.BasicPublish(exchange: string.Empty,
            routingKey: "simulated",
            basicProperties: null,
            body: body);
        
        eventSimulatedCounter.Add(1);
    })
    .WithName("SimulateEvent")
    .WithOpenApi();

app.Run();