using DTA.BPS.Configuration;
using DTA.BPS.Services.Interfaces;
using DTA.Models.Protos.Files;
using Grpc.Net.Client;

namespace DTA.BPS.Services;

public class ProcessingService : IProcessingService
{
    public void GetDataAndProcess(int fileId)
    {
        using var channel = GrpcChannel.ForAddress(ServiceConfiguration.FileServiceAddress);
        var client = new FileServer.FileServerClient(channel);
        
        client.GetFile(new FileRequest { Id = fileId });
        
        // Simulate processing
        Thread.Sleep(3000);
    }
}