namespace DTA.EPS.Api.Rabbit.Interfaces;

/// <summary>
/// Service for RabbitMq
/// </summary>
public interface IRabbitMqService
{
    /// <summary>
    /// Publish a simulated event
    /// </summary>
    /// <param name="id">The id of the event</param>
    void PublishSimulatedEvent(int id);
}