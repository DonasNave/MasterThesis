using DTA.FUS.Monitoring;
using DTA.FUS.Services.Interfaces;

namespace DTA.FUS.Api.Rest;

public static class FileModule
{
    public static void MapFileModule(this WebApplication app)
    {
        app.MapGet("/api/file/download/{id:int}", DownloadFile);
        
        app.MapPost("/api/file/upload", UploadFile).DisableAntiforgery();
    }
    
    private static async Task<IResult> UploadFile(IFormFile file, IFileService fileService)
    {
        AppMonitor.UploadsCounter.Add(1);
        return await fileService.Upload(file);
    }
    
    private static async Task<IResult> DownloadFile(int id, IFileService fileService)
    {
        AppMonitor.DownloadCounter.Add(1);
        return await fileService.Download(id);
    }
}