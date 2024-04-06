using System.Diagnostics.Metrics;

namespace DTA.BPS.Monitoring;

public static class AppMonitor
{
    public static Counter<long> BatchProcessCounter { get; set; } = null!;
}