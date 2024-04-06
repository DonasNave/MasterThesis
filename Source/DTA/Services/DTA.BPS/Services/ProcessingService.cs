using DTA.BPS.Services.Interfaces;
using DTA.Models.Options;
using DTA.Models.Protos.Files;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;

namespace DTA.BPS.Services;

public class ProcessingService(IOptions<ServiceConnectionOptions> options) : IProcessingService
{
    public void GetDataAndProcess(int fileId)
    {
        using var channel = GrpcChannel.ForAddress(options.Value.FusHttp2);
        var client = new FileServer.FileServerClient(channel);
        
        client.GetFile(new FileRequest { Id = fileId });
        
        // Simulate processing
        Thread.Sleep(3000);
    }
}