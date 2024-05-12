using System.Diagnostics.Metrics;
using DTA.BPS.Api.Rabbit;
using DTA.BPS.Monitoring;
using DTA.BPS.Services;
using DTA.BPS.Services.Interfaces;

namespace DTA.BPS.Extensions;

/// <summary>
/// Extensions meant for application initialization
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
        AppMonitor.FilesProcessedCounter = meter.CreateCounter<long>("file_event_consumed_counter");
        AppMonitor.FibonacciProcessedCounter = meter.CreateCounter<long>("fibonacci_processed_counter");
        AppMonitor.PrimesProcessedCounter = meter.CreateCounter<long>("primes_processed_counter");
    }

    /// <summary>
    /// Register the services for the application
    /// </summary>
    /// <param name="serviceCollection">The application service collection</param>
    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<RabbitMqConsumerService>();

        serviceCollection.AddSingleton<IProcessingService, ProcessingService>();
    }
}