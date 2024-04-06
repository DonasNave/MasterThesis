using System.Diagnostics.Metrics;

namespace DTA.EPS.Monitoring;

public static class AppMonitor
{
    public static Counter<long> EventSimulatedCounter { get; set; } = null!;
}