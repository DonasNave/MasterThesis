namespace DTA.Models.Options;

/// <summary>
/// Represents service connection options
/// </summary>
public class ServiceConnectionOptions
{
    public string FusHttp1 { get; set; } = string.Empty;
    public string FusHttp2 { get; set; } = string.Empty;

    public string BpsHttp1 { get; set; } = string.Empty;

    public string EpsHttp1 { get; set; } = string.Empty;

    public string SrsHttp1 { get; set; } = string.Empty;
}