using DTA.EPS.Api.Rabbit.Interfaces;
using DTA.EPS.Monitoring;

namespace DTA.EPS.Api.Rest;

/// <summary>
/// Module for the event API
/// </summary>
public static class EventModule
{
    /// <summary>
    /// Map the event module
    /// </summary>
    public static void MapEventModule(this WebApplication app)
    {
        app.MapGet("/api/simulateEvent/{id:int}", SimulateEvent);
    }

    /// <summary>
    /// Simulate an event for the given id
    /// </summary>
    /// <param name="id">The id of the event</param>
    /// <param name="rabbitMqService">The RabbitMq service injection</param>
    private static IResult SimulateEvent(int id, IRabbitMqService rabbitMqService)
    {
        rabbitMqService.PublishSimulatedEvent(id);
        AppMonitor.EventSimulatedCounter.Add(1);
        return Results.Ok();
    }
}