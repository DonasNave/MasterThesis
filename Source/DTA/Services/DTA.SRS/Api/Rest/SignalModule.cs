using DTA.SRS.Monitoring;
using DTA.SRS.Services.Interfaces;

namespace DTA.SRS.Api.Rest;

public static class SignalModule
{
    public static void MapSignalModule(this WebApplication app)
    {
        app.MapGet("/api/signals/{count:int}", GetRandomSignals);
    }

    private static async Task<IResult> GetRandomSignals(int count, IReadingService readingService)
    {
        var signals = await readingService.GetRandomSignals(count);
        AppMonitor.SignalCounter.Add(1);
        return signals;
    }
}