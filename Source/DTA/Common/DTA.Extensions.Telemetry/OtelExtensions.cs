using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DTA.Extensions.Telemetry;

/// <summary>
/// Extensions meant for application telemetry configuration
/// </summary>
public static class OtelExtensions
{
    /// <summary>
    /// Setup OpenTelemetry configuration, with the provided options. Registers logging, metrics, and tracing.
    /// </summary>
    /// <param name="builder">The web application builder</param>
    /// <param name="optionsAction">Telemetry configuration action</param>
    public static void SetupOpenTelemetry(this IHostApplicationBuilder builder,
        Action<TelemetryConfiguration> optionsAction)
    {
        // Options
        TelemetryConfiguration config = new();
        optionsAction(config);

        var exportProtocol = config.OpenTelemetrySettings.ExporterProtocol switch
        {
            "http" => OtlpExportProtocol.HttpProtobuf,
            "grpc" => OtlpExportProtocol.Grpc,
            _ => throw new ArgumentException("Invalid exporter protocol")
        };

        // Create service Resource Builder
        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(
                serviceName: config.ServiceName,
                serviceVersion: config.ServiceVersion);

        // Logging
        builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(resourceBuilder);

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
                    .SetResourceBuilder(resourceBuilder)
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
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        // Exporters
        builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter(options =>
        {
            options.Endpoint = config.OpenTelemetrySettings.ExporterEndpoint;
            options.Protocol = exportProtocol;
        }));
        builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter(options =>
        {
            options.Endpoint = config.OpenTelemetrySettings.ExporterEndpoint;
            options.Protocol = exportProtocol;

        }));
        builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter(options =>
        {
            options.Endpoint = config.OpenTelemetrySettings.ExporterEndpoint;
            options.Protocol = exportProtocol;
        }));
    }

    /// <summary>
    /// Map default health check endpoints
    /// </summary>
    /// <param name="app">The web application</param>
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