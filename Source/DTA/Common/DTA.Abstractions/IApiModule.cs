using Microsoft.AspNetCore.Builder;

namespace DTA.Abstractions;

/// <summary>
/// Interface for an API module
/// </summary>
public interface IApiModule
{
    /// <summary>
    /// Register the endpoints for the module
    /// </summary>
    /// <param name="app">The application builder</param>
    void RegisterEndpoints(WebApplication app);
}