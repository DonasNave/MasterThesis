using System.Diagnostics.Metrics;

namespace DTA.SRS.Monitoring;

/// <summary>
/// Application monitor class for metrics
/// </summary>
public static class AppMonitor
{
    /// <summary>
    /// The counter for signals requests
    /// </summary>
    public static Counter<long> SignalCounter { get; set; } = null!;
}