namespace DTA.Models;

public abstract class OpenTelemetrySettings
{
    public required Uri ExporterEndpoint { get; init; }
}
