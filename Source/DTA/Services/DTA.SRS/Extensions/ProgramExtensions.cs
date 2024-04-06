using System.Diagnostics.Metrics;
using DTA.Models.JsonSerializers;
using DTA.SRS.Monitoring;
using DTA.SRS.Services.Interfaces;

namespace DTA.SRS.Extensions;

public static class ProgramExtensions
{
    public static void InitializeMetrics(this WebApplication _, string meterName, string serviceVersion)
    {
        var meter = new Meter(meterName, serviceVersion);
        AppMonitor.SignalCounter = meter.CreateCounter<long>("signal_api_calls_counter");
    }
    
    public static void MapMinimalApi(this WebApplication app)
    {
        app.MapGet("/api/signals/{count:int}", async (int count, IReadingService readingService) =>
        {
            var signals = await readingService.GetRandomSignals(count);
            AppMonitor.SignalCounter.Add(1);
            return signals;
        });
    }
    
    public static IServiceCollection RegisterContextSerializers(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, DtaFileContext.Default);
        });
        
        return services;
    }
}