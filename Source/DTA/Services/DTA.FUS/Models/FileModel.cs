namespace FUS.Models;

public class FileModel
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
}