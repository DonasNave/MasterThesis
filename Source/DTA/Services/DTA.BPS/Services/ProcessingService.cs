using DTA.BPS.Configuration;
using DTA.Models.Protos.Files;
using Grpc.Net.Client;

namespace DTA.BPS.Services;

public class ProcessingService : IProcessingService
{
    public void GetDataAndProcess()
    {
        using var channel = GrpcChannel.ForAddress(ServiceConfiguration.FileServiceAddress);
        var client = new FileServer.FileServerClient(channel);
        
        client.GetFile(new FileRequest { Id = 1 });
        
        // Simulate processing
        Thread.Sleep(3000);
    }
}