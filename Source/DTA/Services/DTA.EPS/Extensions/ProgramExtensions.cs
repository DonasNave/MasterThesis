using System.Diagnostics.Metrics;
using DTA.EPS.Api.Rabbit;
using DTA.EPS.Api.Rabbit.Interfaces;
using DTA.EPS.Monitoring;

namespace DTA.EPS.Extensions;

public static class ProgramExtensions
{
    public static void InitializeMetrics(this WebApplication _, string meterName, string serviceVersion)
    {
        var meter = new Meter(meterName, serviceVersion);
        AppMonitor.EventSimulatedCounter = meter.CreateCounter<long>("file_event_produced_counter");
    }

    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IRabbitMqService, RabbitMqService>();
    }
}