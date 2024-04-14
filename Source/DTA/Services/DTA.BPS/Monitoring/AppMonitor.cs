using System.Diagnostics.Metrics;

namespace DTA.BPS.Monitoring;

public static class AppMonitor
{
    public static Counter<long> FilesProcessedCounter { get; set; } = null!;
    public static Counter<long> FibonacciProcessedCounter { get; set; } = null!;
    public static Counter<long> PrimesProcessedCounter { get; set; } = null!;
}