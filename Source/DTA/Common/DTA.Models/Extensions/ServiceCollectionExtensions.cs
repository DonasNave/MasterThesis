using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace DTA.Models.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterContextSerializers(this IServiceCollection services, JsonSerializerContext[] contextFiles)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            for (var index = 0; index < contextFiles.Length; index++)
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(index, contextFiles[index]);
            }
        });
        
        return services;
    }
}