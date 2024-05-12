using System.Diagnostics.Metrics;
using DTA.SRS.Monitoring;
using DTA.SRS.Services;
using DTA.SRS.Services.Interfaces;

namespace DTA.SRS.Extensions;

/// <summary>
/// Extensions meant for application initialization
/// </summary>
public static class ProgramExtensions
{
    /// <summary>
    /// Initialize the metrics for the application
    /// </summary>
    /// <param name="_">The web application</param>
    /// <param name="meterName">The meter name</param>
    /// <param name="serviceVersion">The service version</param>
    public static void InitializeMetrics(this WebApplication _, string meterName, string serviceVersion)
    {
        var meter = new Meter(meterName, serviceVersion);
        AppMonitor.SignalCounter = meter.CreateCounter<long>("signals_rest_calls_counter");
    }

    /// <summary>
    /// Register the services for the application
    /// </summary>
    /// <param name="serviceCollection">The service collection</param>
    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IReadingService, ReadingService>();
    }
}