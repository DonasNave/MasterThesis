using Microsoft.AspNetCore.Builder;

namespace DTA.Abstractions;

public interface IApiModule
{
    void RegisterEndpoints(WebApplication app);
}