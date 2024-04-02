namespace DTA.Models.Signal;

public class DtaSignal
{
    public float Value { get; set; }
    public DateTime Time { get; set; }
    public UnitEnum Unit { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
}