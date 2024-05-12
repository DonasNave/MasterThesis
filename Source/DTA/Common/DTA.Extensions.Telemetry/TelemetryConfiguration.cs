namespace DTA.Extensions.Telemetry;

/// <summary>
/// Configuration for telemetry
/// </summary>
public class TelemetryConfiguration
{
    public OpenTelemetrySettings OpenTelemetrySettings { get; set; } = null!;
    public string ServiceName { get; set; } = null!;
    public string? ServiceVersion { get; set; }
    public string[]? MeterNames { get; set; }
}