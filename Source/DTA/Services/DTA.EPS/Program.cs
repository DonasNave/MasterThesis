using System.Diagnostics.Metrics;
using DTA.Extensions;
using DTA.Models;
using RabbitMQ.Client;

#if AOT
var builder = WebApplication.CreateSlimBuilder(args);
#elif JIT
var builder = WebApplication.CreateBuilder(args);
#endif

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

#if AOT
const string compilationMode = "AOT";
#elif JIT
const string compilationMode = "JIT";
#endif

const string prefix = $"DTA_{compilationMode}_EPS_";
const string serviceName = $"DTA-{compilationMode}-EPS";
const string meterName = $"DTA-{compilationMode}-EPS-Meter";

#if JIT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endif

// Add Environment variables
builder.Configuration.AddEnvironmentVariables(prefix: prefix);

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

var app = builder.Build();

#if JIT
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
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