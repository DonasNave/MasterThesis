using Dapper;
using DTA.FUS.Data;
using DTA.FUS.Services.Interfaces;
using DTA.Models.Files;
using DTA.Models.Response;
using Npgsql;

namespace DTA.FUS.Services;

#if AOT
[DapperAot]
#endif
public class FileService : IFileService
{
    public async Task<IResult> Upload(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return Results.BadRequest("File is empty");

        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
    
        var fileModel = new DtaFile
        {
            FileName = file.FileName,
            Content = stream.ToArray()
        };

        const string sql = """INSERT INTO "Files" ("FileName", "Content") VALUES (@FileName, @Content) RETURNING "Id";""";

        await using var connection = new NpgsqlConnection(DbConfiguration.DefaultConnectionString);
    
        var id = await connection.ExecuteScalarAsync<int>(sql, fileModel);
        return Results.Ok(new CommonResponse { Id = id });
    }

    public async Task<IResult> Download(int id)
    {
        const string sql = """SELECT "Id", "FileName", "Content" FROM "Files" WHERE "Id" = @Id;""";

        await using var connection = new NpgsqlConnection(DbConfiguration.DefaultConnectionString);
    
        var fileModel = await connection.QueryFirstOrDefaultAsync<DtaFile>(sql, new { Id = id });
        if (fileModel == null)
        {
            return Results.NotFound();
        }

        var stream = new MemoryStream(fileModel.Content);
        return Results.Stream(stream, "application/octet-stream", fileModel.FileName);
    }
}