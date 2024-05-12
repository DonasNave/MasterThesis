using System.Diagnostics.Metrics;

namespace DTA.BPS.Monitoring;

/// <summary>
/// Application monitor class for metrics
/// </summary>
public static class AppMonitor
{
    /// <summary>
    /// The counter for files processed
    /// </summary>
    public static Counter<long> FilesProcessedCounter { get; set; } = null!;

    /// <summary>
    /// The counter for fibonacci numbers calculated
    /// </summary>
    public static Counter<long> FibonacciProcessedCounter { get; set; } = null!;

    /// <summary>
    /// The counter for primes requests calculated
    /// </summary>
    public static Counter<long> PrimesProcessedCounter { get; set; } = null!;
}