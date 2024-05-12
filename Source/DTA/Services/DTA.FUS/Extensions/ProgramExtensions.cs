using System.Diagnostics.Metrics;
using DTA.FUS.Api.Grpc;
using DTA.FUS.Monitoring;
using DTA.FUS.Services;
using DTA.FUS.Services.Interfaces;

namespace DTA.FUS.Extensions;

/// <summary>
/// Extensions meant for application initialization
/// </summary>
public static class ProgramExtensions
{
    /// <summary>
    /// Initialize the metrics for the application
    /// </summary>
    public static void InitializeMetrics(this WebApplication _, string meterName, string serviceVersion)
    {
        var meter = new Meter(meterName, serviceVersion);
        AppMonitor.DownloadCounter = meter.CreateCounter<long>("file_download_rest_calls_counter");
        AppMonitor.UploadsCounter = meter.CreateCounter<long>("file_upload_rest_calls_counter");
        AppMonitor.GetFileProceduresCounter = meter.CreateCounter<long>("get_file_procedure_calls_counter");
    }

    /// <summary>
    /// Register the services for the application
    /// </summary>
    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IFileService, FileService>();
    }

    /// <summary>
    /// Map the gRPC services
    /// </summary>
    public static void MapGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<GrpcFileService>();
    }
}