using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DTA.Extensions.Swagger;

public static class ProgramExtensions
{
    public static void AddSwaggerEndpoints(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();
    }
    
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