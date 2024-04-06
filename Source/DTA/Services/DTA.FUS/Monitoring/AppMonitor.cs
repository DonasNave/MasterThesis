using System.Diagnostics.Metrics;

namespace DTA.FUS.Monitoring;

public static class AppMonitor
{
    public static Counter<long> DownloadCounter { get; set; } = null!;
    public static Counter<long> UploadsCounter { get; set; } = null!;
    public static Counter<long> GetFileProceduresCounter { get; set; } = null!;
}