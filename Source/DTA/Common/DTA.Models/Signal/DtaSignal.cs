namespace DTA.Models.Signal;

/// <summary>
/// Represents a signal
/// </summary>
public class DtaSignal
{
    public float Value { get; set; }
    public DateTime Time { get; set; }
    public UnitEnum Unit { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
}