namespace DTA.Models.Files;

/// <summary>
/// Represents a DTA file
/// </summary>
public class DtaFile
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
}