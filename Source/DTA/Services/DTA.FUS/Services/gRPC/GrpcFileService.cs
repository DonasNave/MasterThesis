using DTA.FUS.Services.Interfaces;
using DTA.Models.Protos.Files;
using Grpc.Core;

namespace DTA.FUS.Services.gRPC;

public class GrpcFileService(IFileService fileService) : FileServer.FileServerBase
{
    public override async Task<FileResponse> GetFile(FileRequest request, ServerCallContext context)
    {
        var file = await fileService.GetFile(request.Id);
        
        if (file == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "File not found"));
        }
        
        return new FileResponse
        {
            Id = file.Id,
            FileName = file.FileName,
            FileData = Google.Protobuf.ByteString.CopyFrom(file.Content)
        };
    }
}