using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DTA.Extensions.Telemetry;

public static class OtelExtensions
{
    public static void SetupOpenTelemetry(this IHostApplicationBuilder builder,
        Action<TelemetryConfiguration> optionsAction)
    {
        // Options
        TelemetryConfiguration config = new();
        optionsAction(config);
        
        // Logging
        builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(
                ResourceBuilder
                    .CreateDefault()
                    .AddService(
                        serviceName: config.ServiceName,
                        serviceVersion: config.ServiceVersion,
                        serviceInstanceId: Environment.MachineName));

            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;
        });
        
        // Metrics
        string[] defaultMeters =
            ["System.Runtime", "System.Net.Http", "Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", "Npgsql"];

        if (config.MeterNames is { Length: > 0 } meterNames)
            defaultMeters = meterNames.Union(defaultMeters).ToArray();
        
        builder.Services.AddOpenTelemetry()
            // Metrics
            .WithMetrics(options =>
            {
                options
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter(defaultMeters);
            })
            // Tracing
            .WithTracing(options =>
            {
                if (builder.Environment.IsDevelopment())
                    options.SetSampler<AlwaysOnSampler>();
                
                options
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql();
            });

        // Exporters
        builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter(options => {
            options.Endpoint = config.OpenTelemetrySettings.ExporterEndpoint;
            options.Protocol = OtlpExportProtocol.Grpc;
        }));
        builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter(options =>
        {
            options.Endpoint = config.OpenTelemetrySettings.ExporterEndpoint;
            options.Protocol = OtlpExportProtocol.Grpc;
        
        }));
        builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter(options =>
        {
            options.Endpoint = config.OpenTelemetrySettings.ExporterEndpoint;
            options.Protocol = OtlpExportProtocol.Grpc;
        }));
    }
    
    public static void MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        
        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("live"),
        });
        
        app.MapHealthChecks("/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("ready"),
        });
    }
}