﻿using Microsoft.AspNetCore.Builder;

namespace DTA.Extensions.Common;

public static class ProgramExtensions
{
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