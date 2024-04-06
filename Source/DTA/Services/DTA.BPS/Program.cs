using System.Diagnostics.Metrics;
using System.Text;
using DTA.BPS.Configuration;
using DTA.BPS.Services.Interfaces;
using DTA.Extensions.Common;
using DTA.Extensions.Telemetry;
using DTA.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

#if DEBUG_JIT
using DTA.Extensions.Swagger;
#endif

#if AOT
var builder = WebApplication.CreateSlimBuilder(args);
#else
var builder = WebApplication.CreateBuilder(args);
#endif

builder.WithServiceNames(out var serviceName, out var meterName);

// Add Environment variables
builder.Configuration.AddEnvironmentVariables(prefix: serviceName);

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var fusUrl = builder.Configuration["ServiceConnections:FUS:Url"];

if (string.IsNullOrWhiteSpace(fusUrl))
{
    throw new InvalidOperationException("File service URL is missing");
}

ServiceConfiguration.FileServiceAddress = fusUrl;

#if DEBUG_JIT
builder.Services.AddSwaggerEndpoints();
#endif

var app = builder.Build();

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

#if DEBUG_JIT
app.SetupSwagger();
#endif

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "simulated",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (_, ea) =>
{
    var body = ea.Body.ToArray();
    var fileId = int.Parse(Encoding.UTF8.GetString(body));
    
    var processingService = Activator.CreateInstance<IProcessingService>();
    processingService.GetDataAndProcess(fileId);
    batchCounter.Add(1);
};

channel.BasicConsume(queue: "simulated",
    autoAck: true,
    consumer: consumer);

app.Run();