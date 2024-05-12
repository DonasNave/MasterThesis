using DTA.FUS.Monitoring;
using DTA.FUS.Services.Interfaces;

namespace DTA.FUS.Api.Rest;

/// <summary>
/// Module for the file API
/// </summary>
public static class FileModule
{
    /// <summary>
    /// Map the file module
    /// </summary>
    /// <param name="app">The application builder</param>
    public static void MapFileModule(this WebApplication app)
    {
        app.MapGet("/api/file/download/{id:int}", DownloadFile);

        app.MapPost("/api/file/upload", UploadFile).DisableAntiforgery();
    }

    /// <summary>
    /// Handle the file upload
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="fileService">The file service injection</param>
    /// <returns>The result of the upload</returns>
    private static async Task<IResult> UploadFile(IFormFile file, IFileService fileService)
    {
        AppMonitor.UploadsCounter.Add(1);
        return await fileService.Upload(file);
    }

    /// <summary>
    /// Handle the file download
    /// </summary>
    /// <param name="id">The id of the file to download</param>
    /// <param name="fileService">The file service injection</param>
    /// <returns>The result of the download</returns>
    private static async Task<IResult> DownloadFile(int id, IFileService fileService)
    {
        AppMonitor.DownloadCounter.Add(1);
        return await fileService.Download(id);
    }
}