using System.Reflection;
using DTA.Models;

namespace DTA.Shared.Models;

public class TelemetryConfiguration
{
    public OpenTelemetrySettings OpenTelemetrySettings { get; set; } = null!;
    public string ServiceName { get; set; } = null!;
    public string? ServiceVersion { get; set; }
    public string[]? MeterNames { get; set; }
}