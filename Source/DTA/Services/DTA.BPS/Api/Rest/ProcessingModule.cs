using DTA.BPS.Monitoring;
using DTA.BPS.Services.Interfaces;
using DTA.Extensions.Telemetry;
using DTA.Models.Response;

namespace DTA.BPS.Api.Rest;

/// <summary>
/// Module for processing requests
/// </summary>
public static class ProcessingModule
{
    /// <summary>
    /// Maps the processing module
    /// </summary>
    public static void MapProcessingModule(this WebApplication app)
    {
        app.MapGet("/api/processFibonacci/{degree:int}", ProcessFibonacci);
        app.MapGet("/api/processPrimeFactors/{number:long}", ProcessPrimeFactors);
    }

    /// <summary>
    /// Processes a Fibonacci request
    /// </summary>
    /// <param name="request">The HTTP request</param>
    /// <param name="degree">The degree</param>
    /// <param name="processingService">The processing service injection</param>
    private static IResult ProcessFibonacci(HttpRequest request, int degree, IProcessingService processingService)
    {
        var result = processingService.Fibonacci(degree);
        AppMonitor.FibonacciProcessedCounter.Add(1, request.GetTestTags());
        return Results.Ok(new FibonacciResponse { Result = result });
    }

    /// <summary>
    /// Processes a Prime Factors request
    /// </summary>
    /// <param name="request">The HTTP request</param>
    /// <param name="number">The number to be processed</param>
    /// <param name="processingService">The processing service injection</param>
    private static IResult ProcessPrimeFactors(HttpRequest request, long number, IProcessingService processingService)
    {
        var result = processingService.PrimeFactors(number);
        AppMonitor.PrimesProcessedCounter.Add(1, request.GetTestTags());
        return Results.Ok(new PrimesReponse { Primes = result });
    }
}