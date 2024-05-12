using DTA.EPS.Api.Rabbit.Interfaces;
using DTA.Models.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DTA.EPS.Api.Rabbit;

/// <summary>
/// Represents a RabbitMq service
/// </summary>
public class RabbitMqService : IDisposable, IRabbitMqService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    /// <summary>
    /// Initializes a new instance of <see cref="RabbitMqService"/>
    /// </summary>
    /// <param name="options">The RabbitMq configuration action</param>
    public RabbitMqService(IOptions<RabbitMqOptions> options)
    {
        var factory = new ConnectionFactory { HostName = options.Value.HostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _queueName = $"{options.Value.QueueGroup}_simulated";

        _channel.QueueDeclare(queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: true,
            arguments: null);
    }

    /// <inheritdoc />
    public void PublishSimulatedEvent(int id)
    {
        var body = BitConverter.GetBytes(id);

        _channel.BasicPublish(exchange: string.Empty,
            routingKey: _queueName,
            basicProperties: null,
            body: body);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}