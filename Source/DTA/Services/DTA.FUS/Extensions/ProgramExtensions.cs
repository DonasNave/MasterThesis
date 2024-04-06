using System.Diagnostics.Metrics;
using DTA.FUS.Monitoring;
using DTA.FUS.Services.gRPC;
using DTA.FUS.Services.Interfaces;
using DTA.Models.JsonSerializers;

namespace DTA.FUS.Extensions;

public static class ProgramExtensions
{
    public static void InitializeMetrics(this WebApplication _, string meterName, string serviceVersion)
    {
        var meter = new Meter(meterName, serviceVersion);
        AppMonitor.DownloadCounter = meter.CreateCounter<long>("download_api_calls_counter");
        AppMonitor.UploadsCounter = meter.CreateCounter<long>("upload_api_calls_counter");
        AppMonitor.GetFileProceduresCounter = meter.CreateCounter<long>("get_file_procedure_calls_counter");
    }
    
    public static void MapMinimalApi(this WebApplication app)
    {
        app.MapPost("/api/file/upload", async (IFormFile file, IFileService fileService) =>
        {
            AppMonitor.UploadsCounter.Add(1);
            return await fileService.Upload(file);
        }).DisableAntiforgery();

        app.MapGet("/api/file/download/{id:int}", async (int id, IFileService fileService) =>
        {
            AppMonitor.DownloadCounter.Add(1);
            return await fileService.Download(id);
        });
    }
    
    public static void MapGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<GrpcFileService>();
    }
    
    public static IServiceCollection RegisterContextSerializers(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, CommonResponseContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Insert(1, DtaFileContext.Default);
        });
        
        return services;
    }
}