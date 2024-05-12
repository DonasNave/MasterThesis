using System.Diagnostics.Metrics;

namespace DTA.EPS.Monitoring;

/// <summary>
/// Application monitor class for metrics
/// </summary>
public static class AppMonitor
{
    /// <summary>
    /// The counter for events simulated
    /// </summary>
    public static Counter<long> EventSimulatedCounter { get; set; } = null!;
}