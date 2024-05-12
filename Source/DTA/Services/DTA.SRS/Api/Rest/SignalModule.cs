using DTA.SRS.Monitoring;
using DTA.SRS.Services.Interfaces;

namespace DTA.SRS.Api.Rest;

/// <summary>
/// Module for the signal API
/// </summary>
public static class SignalModule
{
    /// <summary>
    /// Map the signal module
    /// </summary>
    /// <param name="app">The application builder</param>
    public static void MapSignalModule(this WebApplication app)
    {
        app.MapGet("/api/signals/{count:int}", GetRandomSignals);
    }

    /// <summary>
    /// Handle the random signals request
    /// </summary>
    /// <param name="count">The number of signals to return</param>
    /// <param name="readingService">The reading service injection</param>
    private static async Task<IResult> GetRandomSignals(int count, IReadingService readingService)
    {
        var signals = await readingService.GetRandomSignals(count);
        AppMonitor.SignalCounter.Add(1);
        return signals;
    }
}