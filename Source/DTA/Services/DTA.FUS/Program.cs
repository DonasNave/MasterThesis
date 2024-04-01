using System.Diagnostics.Metrics;
using DTA.Extensions;
using FUS.Data;
using FUS.Services;
using FUS.Services.Interfaces;
using DTA.Models;

#if JIT
using Microsoft.EntityFrameworkCore;
#endif

#if AOT
var builder = WebApplication.CreateSlimBuilder(args);
#elif JIT
var builder = WebApplication.CreateBuilder(args);
#endif

// Setup logging to console
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.
#if AOT
const string prefix = "DTA_AOT_FUS_";
const string serviceName = "DTA-AOT-FUS";
const string meterName = "DTA-AOT-FUS-Meter";
#else
const string prefix = "DTA_JIT_FUS_";
const string serviceName = "DTA-JIT-FUS";
const string meterName = "DTA-JIT-FUS-Meter";
#endif

builder.Configuration.AddEnvironmentVariables(prefix: prefix);

builder.Services.AddHealthChecks();

var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

#if JIT
builder.Services.AddDbContext<FileContext>(options =>
    options.UseNpgsql(dbConnectionString));
#elif AOT
DbConfiguration.DefaultConnectionString = dbConnectionString;
builder.Services.AddScoped<FileContext>();
#endif

builder.Services.AddTransient<IFileService, FileService>();

var settings =
    builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>()!;

#if AOT
const string serviceVersion = "1.0.0";
#elif JIT
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";
#endif

// Add OpenTelemetry ...
builder.SetupOpenTelemetry(options =>
{
    options.OpenTelemetrySettings = settings;
    options.ServiceName = serviceName;
    options.ServiceVersion = serviceVersion;
    options.MeterNames = [meterName];
});

#if JIT
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endif

var meter = new Meter(meterName, serviceVersion);
var counterUploads = meter.CreateCounter<long>("download_api_calls_counter");
var counterDownloads = meter.CreateCounter<long>("upload_api_calls_counter");

var app = builder.Build();

#if JIT
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Swagger redirect
    app.MapGet("/", context => 
    {
        context.Response.Redirect("/swagger/index.html");
        return Task.CompletedTask;
    });
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<FileContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
    }
}
#endif

app.MapDefaultEndpoints();

app.MapPost("/api/file/upload", async (IFormFile file, IFileService fileService) =>
{
    counterUploads.Add(1);
    return await fileService.Upload(file);
}).DisableAntiforgery();

app.MapGet("/api/file/download/{id:int}", async (int id, IFileService fileService) =>
{
    counterDownloads.Add(1);
    return await fileService.Download(id);
});

app.Run();
