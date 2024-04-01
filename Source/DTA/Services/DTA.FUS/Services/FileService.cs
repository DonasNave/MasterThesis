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

        int fileId;
        
#if AOT
        //WORKAROUND: Npgsql doesn't support data insert via compiled models, thus we use raw SQL
        await using (var command = context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = $"""INSERT INTO "Files" ("FileName", "Content") VALUES (@FileName, @Content) RETURNING "Id";""";
            await context.Database.OpenConnectionAsync();
            
            var fileNameParam = command.CreateParameter();
            fileNameParam.ParameterName = "@FileName";
            fileNameParam.Value = fileModel.FileName;
            command.Parameters.Add(fileNameParam);

            var contentParam = command.CreateParameter();
            contentParam.ParameterName = "@Content";
            contentParam.Value = fileModel.Content;
            command.Parameters.Add(contentParam);
            
            var result = await command.ExecuteScalarAsync();
            
            if (result == null || result == DBNull.Value)
                return Results.BadRequest("Failed to upload file");
            
            fileId = (int)result;
        }
#elif JIT
        context.Files?.Add(fileModel);
        await context.SaveChangesAsync();
        fileId = fileModel.Id;
#endif

        return Results.Ok(fileId);
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