using System.Diagnostics.Metrics;
using DTA.BPS.Api.Rabbit;
using DTA.BPS.Monitoring;
using DTA.BPS.Services;
using DTA.BPS.Services.Interfaces;

namespace DTA.BPS.Extensions;

public static class ProgramExtensions
{
    public static void InitializeMetrics(this WebApplication _, string meterName, string serviceVersion)
    {
        var meter = new Meter(meterName, serviceVersion);
        AppMonitor.BatchProcessCounter = meter.CreateCounter<long>("batch_event_consumed_counter");
    }

    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<RabbitMqConsumerService>();
        
        serviceCollection.AddSingleton<IProcessingService, ProcessingService>();
    }
}