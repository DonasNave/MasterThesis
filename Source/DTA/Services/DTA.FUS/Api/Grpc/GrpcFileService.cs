using DTA.FUS.Monitoring;
using DTA.FUS.Services.Interfaces;
using DTA.Models.Protos.Files;
using Grpc.Core;

namespace DTA.FUS.Api.Grpc;

/// <summary>
/// Grpc service for file operations
/// </summary>
public class GrpcFileService(IFileService fileService) : FileServer.FileServerBase
{
    /// <summary>
    /// Handle the get file call
    /// </summary>
    /// <param name="request">Request for the file</param>
    /// <param name="context">The server call context</param>
    /// <returns>Returns the file response</returns>
    /// <exception cref="RpcException">Throws an exception if the file is not found</exception>
    public override async Task<FileResponse> GetFile(FileRequest request, ServerCallContext context)
    {
        var file = await fileService.GetFile(request.Id);

        if (file == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "File not found"));
        }

        AppMonitor.GetFileProceduresCounter.Add(1);

        return new FileResponse
        {
            Id = file.Id,
            FileName = file.FileName,
            FileData = Google.Protobuf.ByteString.CopyFrom(file.Content)
        };
    }
}