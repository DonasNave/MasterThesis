using DTA.EPS.Api.Rabbit.Interfaces;
using DTA.EPS.Monitoring;

namespace DTA.EPS.Api.Rest;

public static class EventModule
{
    public static void MapEventModule(this WebApplication app)
    {
        app.MapGet("/api/simulateEvent/{id:int}", SimulateEvent);
    }
    
    private static IResult SimulateEvent(int id, IRabbitMqService rabbitMqService)
    {
        rabbitMqService.PublishSimulatedEvent(id);
        AppMonitor.EventSimulatedCounter.Add(1);
        return Results.Ok();
    }
}