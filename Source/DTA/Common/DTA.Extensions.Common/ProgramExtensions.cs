using Microsoft.AspNetCore.Builder;

namespace DTA.Extensions.Common;

/// <summary>
/// Extensions meant for application configuration
/// </summary>
public static class ProgramExtensions
{
        /// <summary>
        /// Get the service names for the application
        /// </summary>
        /// <param name="builder">The web application builder</param>
        /// <param name="serviceName">The service name</param>
        /// <param name="meterName">The meter name</param>
        public static void WithServiceNames(this WebApplicationBuilder builder, out string serviceName, out string meterName)
        {
#if AOT
                const string compilationMode = "AOT";
#elif JIT
                const string compilationMode = "JIT";
#endif
                const string prefix = $"DTA_{compilationMode}_";
                serviceName = $"{prefix}{builder.Environment.ApplicationName}";
                meterName = $"{serviceName}-Meter";
        }
}