using DTA.BPS.Monitoring;
using DTA.BPS.Services.Interfaces;
using DTA.Models.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DTA.BPS.Api.Rabbit;

public class RabbitMqConsumerService : IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly IProcessingService _processingService;

    public RabbitMqConsumerService(IOptions<RabbitMqOptions> options, IProcessingService processingService)
    {
        _options = options.Value;
        _processingService = processingService;
        InitializeConsumer();
    }

    private void InitializeConsumer()
    {
        var factory = new ConnectionFactory { HostName = _options.HostName };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "simulated",
            durable: true,
            exclusive: false,
            autoDelete: true,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += OnMessageReceived;

        channel.BasicConsume(queue: "simulated",
            autoAck: true,
            consumer: consumer);
    }

    private void OnMessageReceived(object? model, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var fileId = BitConverter.ToInt32(body);
        
        _processingService.GetDataAndProcess(fileId);
        AppMonitor.BatchProcessCounter.Add(1);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}