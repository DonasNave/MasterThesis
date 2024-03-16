using DTA.Shared.Models;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DTA.JIT.Extensions;

public static class OtelExtensions
{
    public static void SetupMyOtelLogging(this ILoggingBuilder builder, OpenTelemetrySettings settings, string serviceName, string serviceVersion)
    {
        builder.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(
                ResourceBuilder
                    .CreateDefault()
                    .AddService(
                        serviceName: serviceName,
                        serviceVersion: serviceVersion,
                        serviceInstanceId: Environment.MachineName));

            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;

            options.AddOtlpExporter(exporterOptions =>
            {
                exporterOptions.Endpoint = settings.Endpoint;
            });
        });
    }

    public static OpenTelemetryBuilder SetupMyOtelTracing(this OpenTelemetryBuilder builder, OpenTelemetrySettings settings)
    {
        builder.WithTracing(tracingBuilder =>
        {
            tracingBuilder
                .AddAspNetCoreInstrumentation()
                .AddNpgsql()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = settings.Endpoint;
                    options.Protocol = OtlpExportProtocol.Grpc;
                });
        });

        return builder;
    }

    public static OpenTelemetryBuilder SetupMyOtelMetrics(this OpenTelemetryBuilder builder, OpenTelemetrySettings settings,
        IEnumerable<string>? meterNames = null)
    {
        string[] defaultMeters =
            ["System.Runtime", "Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", "Npgsql"];

        if (meterNames is not null)
            defaultMeters = meterNames.Union(defaultMeters).ToArray();

        builder.WithMetrics(metricsBuilder =>
        {
            metricsBuilder
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter(defaultMeters)
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = settings.Endpoint;
                    options.Protocol = OtlpExportProtocol.Grpc;
                });
        });

        return builder;
    }
}