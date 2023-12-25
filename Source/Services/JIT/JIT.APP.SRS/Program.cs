using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
// Signal Readings Service

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add OpenTelemetry
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

app.Run();
