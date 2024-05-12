using System.Diagnostics.Metrics;

namespace DTA.FUS.Monitoring;

/// <summary>
/// Application monitor class for metrics
/// </summary>
public static class AppMonitor
{
    /// <summary>
    /// The counter for file downloads
    /// </summary>
    public static Counter<long> DownloadCounter { get; set; } = null!;

    /// <summary>
    /// The counter for file uploads
    /// </summary>
    public static Counter<long> UploadsCounter { get; set; } = null!;

    /// <summary>
    /// The counter for get file gRPC procedures
    /// </summary>
    public static Counter<long> GetFileProceduresCounter { get; set; } = null!;
}