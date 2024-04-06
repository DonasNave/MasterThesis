using System.Diagnostics.Metrics;

namespace DTA.SRS.Monitoring;

public static class AppMonitor
{
    public static Counter<long> SignalCounter { get; set; } = null!;
}