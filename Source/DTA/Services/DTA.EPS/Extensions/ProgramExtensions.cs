using System.Diagnostics.Metrics;
using DTA.EPS.Api.Rabbit;
using DTA.EPS.Api.Rabbit.Interfaces;
using DTA.EPS.Monitoring;

namespace DTA.EPS.Extensions;

/// <summary>
/// Extensions meant for application initialization
/// </summary>
public static class ProgramExtensions
{
    /// <summary>
    /// Initialize the metrics for the application
    /// </summary>
    /// <param name="_">The web application builder</param>
    /// <param name="meterName">The meter name</param>
    /// <param name="serviceVersion">The service version</param>
    public static void InitializeMetrics(this WebApplication _, string meterName, string serviceVersion)
    {
        var meter = new Meter(meterName, serviceVersion);
        AppMonitor.EventSimulatedCounter = meter.CreateCounter<long>("file_event_produced_counter");
    }

    /// <summary>
    /// Register the services for the application
    /// </summary>
    /// <param name="serviceCollection">The application service collection</param>
    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IRabbitMqService, RabbitMqService>();
    }
}