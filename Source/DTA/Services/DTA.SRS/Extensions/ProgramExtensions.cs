using System.Diagnostics.Metrics;
using DTA.SRS.Monitoring;
using DTA.SRS.Services;
using DTA.SRS.Services.Interfaces;

namespace DTA.SRS.Extensions;

public static class ProgramExtensions
{
    public static void InitializeMetrics(this WebApplication _, string meterName, string serviceVersion)
    {
        var meter = new Meter(meterName, serviceVersion);
        AppMonitor.SignalCounter = meter.CreateCounter<long>("signals_rest_calls_counter");
    }
    
    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IReadingService, ReadingService>();
    }
}