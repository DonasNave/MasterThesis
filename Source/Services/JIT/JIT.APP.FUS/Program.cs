using FUS.Data;
using FUS.Services;
using FUS.Services.Interfaces;
using JIT.APP.Extensions;
using JIT.APP.Models;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
// File Upload Service

// Add services to the container.

builder.Services.AddDbContext<FileContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IFileService, FileService>();

var settings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;
const string serviceName = "JIT-FUS";
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

// Add OpenTelemetry ...
builder.Services
    .AddOpenTelemetry()
    // ... Resource ...
    .ConfigureResource(resourceBuilder =>
    {
        resourceBuilder.AddService(
            serviceName: serviceName,
            serviceVersion: serviceVersion,
            serviceInstanceId: Environment.MachineName);
    })
    // ... Tracing ...
    .SetupMyOtelTracing(settings)
    // ... Metrics 
    .SetupMyOtelMetrics(settings);

// Add OpenTelemetry Logging
builder.Logging.SetupMyOtelLogging(settings, serviceName, serviceVersion);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/file/upload", async (IFormFile file, IFileService fileService) =>
    await fileService.Upload(file));

app.MapGet("/api/file/download/{id:int}", async (int id, IFileService fileService) =>
    await fileService.Download(id));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
