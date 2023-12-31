using JIT.APP.Models;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SRS.Services;
using SRS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
// Signal Readings Service

// Add services to the container.
builder.Services.AddTransient<IReadingService, ReadingService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add OpenTelemetry ...
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resourceBuilder =>
    {
        resourceBuilder.AddService(
            serviceName: "JIT-SRS",
            serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown",
            serviceInstanceId: Environment.MachineName);
    })
    // ... Tracing ...
    .WithTracing(providerBuilder =>
    {
        providerBuilder
            .AddAspNetCoreInstrumentation()
            .AddNpgsql()
            .AddOtlpExporter(options =>
            {
                options.Endpoint =
                    builder
                        .Configuration
                        .GetSection(nameof(OpenTelemetrySettings))
                        .Get<OpenTelemetrySettings>()!
                        .Endpoint;

                options.Protocol = OtlpExportProtocol.Grpc;
            });
    })
    // ... Metrics 
    .WithMetrics(metrics =>
    {
        metrics
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddMeter(
                "System.Runtime",
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Server.Kestrel",
                "Npgsql")
            .AddOtlpExporter(options =>
            {
                options.Endpoint =
                    builder
                        .Configuration
                        .GetSection(nameof(OpenTelemetrySettings))
                        .Get<OpenTelemetrySettings>()!
                        .Endpoint;

                options.Protocol = OtlpExportProtocol.Grpc;
            });
    });

// Add OpenTelemetry Logging
builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(
        ResourceBuilder
            .CreateDefault()
            .AddService(
                serviceName: "JIT-SRS",
                serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown",
                serviceInstanceId: Environment.MachineName));

    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
    options.ParseStateValues = true;

    options.AddOtlpExporter(exporterOptions =>
    {
        exporterOptions.Endpoint =
            builder
                .Configuration
                .GetSection(nameof(OpenTelemetrySettings))
                .Get<OpenTelemetrySettings>()!
                .Endpoint;
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.MapGet("/api/signals/{count:int}", async (int count, IReadingService readingService) =>
    await readingService.GetRandomSignals(count));

app.Run();
