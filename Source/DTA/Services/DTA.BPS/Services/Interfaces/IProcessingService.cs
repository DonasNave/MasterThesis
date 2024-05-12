namespace DTA.BPS.Services.Interfaces;

/// <summary>
/// Interface for processing service
/// </summary>
public interface IProcessingService
{
    /// <summary>
    /// Gets file data from FUS and processes
    /// </summary>
    /// <param name="fileId">Id of file to be processed</param>
    void GetDataAndProcess(int fileId);

    /// <summary>
    /// Counts n-th Fibonacci number
    /// </summary>
    /// <param name="n">Number position</param>
    long Fibonacci(int n);

    /// <summary>
    /// Processes a Prime Factors for a number
    /// </summary>
    /// <param name="n">Number to be processed</param>
    List<long> PrimeFactors(long n);
}