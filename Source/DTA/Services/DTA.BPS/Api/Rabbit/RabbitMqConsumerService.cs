using System.Text;
using DTA.BPS.Monitoring;
using DTA.BPS.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DTA.BPS.Api.Rabbit;

public class RabbitMqConsumerService : IDisposable
{
    private readonly IProcessingService _processingService; // Assuming you have this from the previous example

    public RabbitMqConsumerService(IProcessingService processingService)
    {
        _processingService = processingService;
        InitializeConsumer();
    }

    private void InitializeConsumer()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "simulated",
            durable: false,
            exclusive: false,
            autoDelete: false,
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
        var fileId = int.Parse(Encoding.UTF8.GetString(body));
        
        _processingService.GetDataAndProcess(fileId);
        AppMonitor.BatchProcessCounter.Add(1);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}