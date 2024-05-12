using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DTA.Extensions.Swagger;

/// <summary>
/// Extensions meant for application configuration
/// </summary>
public static class ProgramExtensions
{
    /// <summary>
    /// Add the swagger endpoints
    /// </summary>
    /// <param name="serviceCollection">The application service collection</param>
    public static void AddSwaggerEndpoints(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();
    }

    /// <summary>
    /// Setup the swagger UI
    /// </summary>
    /// <param name="app">The web application builder</param>
    public static void SetupSwagger(this WebApplication app)
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
}