namespace DTA.EPS.Api.Rabbit.Interfaces;

public interface IRabbitMqService
{
    void PublishSimulatedEvent(int id);
}