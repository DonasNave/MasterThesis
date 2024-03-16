namespace FUS.Services.Interfaces;

public interface IFileService
{
    Task<IResult> Upload(IFormFile? file);
    Task<IResult> Download(int id);
}