namespace DTA.Models.Files;

public class DtaFile
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
}