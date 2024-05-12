namespace DTA.Extensions.Telemetry;

public class OpenTelemetrySettings
{
    public Uri? ExporterEndpoint { get; init; }
    public string ExporterProtocol { get; init; } = "grpc";
    public string ServiceNameSuffix { get; init; } = string.Empty;
}
