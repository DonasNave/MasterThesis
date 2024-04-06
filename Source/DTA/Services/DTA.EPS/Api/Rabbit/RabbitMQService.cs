using DTA.EPS.Api.Rabbit.Interfaces;
using RabbitMQ.Client;

namespace DTA.EPS.Api.Rabbit;

public class RabbitMqService : IDisposable, IRabbitMqService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "simulated",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void PublishSimulatedEvent(int id)
    {
        var body = BitConverter.GetBytes(id);

        _channel.BasicPublish(exchange: string.Empty,
            routingKey: "simulated",
            basicProperties: null,
            body: body);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}