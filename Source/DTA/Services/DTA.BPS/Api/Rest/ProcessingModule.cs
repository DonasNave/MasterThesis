using DTA.BPS.Monitoring;
using DTA.BPS.Services.Interfaces;
using DTA.Models.Response;

namespace DTA.BPS.Api.Rest;

public static class ProcessingModule
{
    public static void MapProcessingModule(this WebApplication app)
    {
        app.MapGet("/api/processFibonacci/{degree:int}", ProcessFibonacci);
        app.MapGet("/api/processPrimeFactors/{number:long}", ProcessPrimeFactors);
    }

    private static IResult ProcessFibonacci(int degree, IProcessingService processingService)
    {
        var result = processingService.Fibonacci(degree);
        AppMonitor.FibonacciProcessedCounter.Add(1);
        return Results.Ok(new FibonacciResponse { Result = result });
    }

    private static IResult ProcessPrimeFactors(long number, IProcessingService processingService)
    {
        var result = processingService.PrimeFactors(number);
        AppMonitor.PrimesProcessedCounter.Add(1);
        return Results.Ok(new PrimesReponse { Primes = result });
    }
}