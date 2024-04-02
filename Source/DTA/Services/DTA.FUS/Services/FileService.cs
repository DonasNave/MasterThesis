using DTA.Models.Files;
using FUS.Data;
using FUS.Services.Interfaces;

#if AOT
using Microsoft.EntityFrameworkCore;
#endif

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
        
#if AOT
        
#elif JIT
        context.Files?.Add(fileModel);
#endif
        await context.SaveChangesAsync();

        return Results.Ok();
    }

    public async Task<IResult> Download(int id)
    {
        if (context.Files == null) return Results.NotFound();
        
        //Aot requires AsNoTracking, as tracking changes uses runtime code generation/model building
#if AOT
        var fileModel = await context.Files.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
#elif JIT
        var fileModel = await context.Files.FindAsync(id);
#endif
        
        if (fileModel == null)
            return Results.NotFound();

        var stream = new MemoryStream(fileModel.Content);
        return Results.Stream(stream, "application/octet-stream", fileModel.FileName);
    }
}