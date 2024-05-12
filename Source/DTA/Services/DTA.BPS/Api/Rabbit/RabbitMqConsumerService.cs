using DTA.BPS.Monitoring;
using DTA.BPS.Services.Interfaces;
using DTA.Models.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DTA.BPS.Api.Rabbit;

/// <summary>
/// Represents a RabbitMq consumer service
/// </summary>
public class RabbitMqConsumerService : IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly IProcessingService _processingService;
    private readonly string _queueName;

    /// <summary>
    /// Initializes a new instance of <see cref="RabbitMqConsumerService"/>
    /// </summary>
    /// <param name="options">The RabbitMq options</param>
    /// <param name="processingService">The processing service</param>
    public RabbitMqConsumerService(IOptions<RabbitMqOptions> options, IProcessingService processingService)
    {
        _options = options.Value;
        _processingService = processingService;
        _queueName = $"{options.Value.QueueGroup}_simulated";
        InitializeConsumer();
    }

    /// <summary>
    /// Initializes the consumer
    /// </summary>
    private void InitializeConsumer()
    {
        var factory = new ConnectionFactory { HostName = _options.HostName };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: true,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += OnMessageReceived;

        channel.BasicConsume(queue: _queueName,
            autoAck: true,
            consumer: consumer);
    }

    /// <summary>
    /// Handles the message received event
    /// </summary>
    /// <param name="model">The model</param>
    /// <param name="ea">The basic deliver event arguments</param>
    private void OnMessageReceived(object? model, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var fileId = BitConverter.ToInt32(body);

        _processingService.GetDataAndProcess(fileId);
        AppMonitor.FilesProcessedCounter.Add(1);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}