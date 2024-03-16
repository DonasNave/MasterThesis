using FUS.Data;
using FUS.Models;
using FUS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FUS.Services;

public class FileService(FileContext context) : IFileService
{
    public async Task<IResult> Upload(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return Results.BadRequest("File is empty");

        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var fileModel = new FileModel
        {
            FileName = file.FileName,
            Content = stream.ToArray()
        };

        context.Files.Add(fileModel);
        await context.SaveChangesAsync();

        return Results.Ok(new { fileModel.Id });
    }

    public async Task<IResult> Download(int id)
    {
        var fileModel = await context.Files.FindAsync(id);

        if (fileModel == null)
            return Results.NotFound();

        var stream = new MemoryStream(fileModel.Content);
        return Results.Ok(new FileStreamResult(stream, "application/octet-stream")
        {
            FileDownloadName = fileModel.FileName
        });
    }
}