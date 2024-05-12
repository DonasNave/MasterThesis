namespace DTA.SRS.Services.Interfaces;

/// <summary>
/// Interface for the reading service
/// </summary>
public interface IReadingService
{
    /// <summary>
    /// Get a number of random signals
    /// </summary>
    /// <param name="count">The number of signals to get</param>
    /// <returns>Result containing signals</returns>
    Task<IResult> GetRandomSignals(int count);
}