using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace DTA.Models.Extensions;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the context serializers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="contextFiles">The context files</param>
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