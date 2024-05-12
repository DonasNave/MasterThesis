using DTA.Models.Files;

namespace DTA.FUS.Services.Interfaces;

/// <summary>
/// Interface for the file service
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Upload provided file
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <returns>The result of the upload</returns>
    Task<IResult> Upload(IFormFile? file);

    /// <summary>
    /// Download specified file
    /// </summary>
    /// <param name="id">The id of the file to download</param>
    /// <returns>The result of the download</returns>
    Task<IResult> Download(int id);

    /// <summary>
    /// Get a file from the database
    /// </summary>
    /// <param name="id">The id of the file to get</param>
    /// <returns>The file</returns>
    /// <remarks>Returns null if the file is not found</remarks>
    Task<DtaFile?> GetFile(int id);
}