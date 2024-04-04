using DTA.Models.Files;

namespace DTA.FUS.Services.Interfaces;

public interface IFileService
{
    Task<IResult> Upload(IFormFile? file);
    Task<IResult> Download(int id);
    Task<DtaFile?> GetFile(int id);
}