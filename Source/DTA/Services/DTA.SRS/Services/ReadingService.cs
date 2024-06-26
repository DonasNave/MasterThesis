using DTA.SRS.Helpers;
using DTA.SRS.Services.Interfaces;

namespace DTA.SRS.Services;

/// <summary>
/// Service for reading signals
/// </summary>
public class ReadingService : IReadingService
{
    /// <inheritdoc />
    public async Task<IResult> GetRandomSignals(int count)
    {
        try
        {
            var signals = await SignalGenerator.GenerateRandomSignalsAsync(count);
            return Results.Ok(signals);
        }
        catch (Exception ex)
        {
            return Results.Problem("Error generating random signals", ex.Message, 500);
        }
    }
}