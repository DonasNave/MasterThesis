using DTA.BPS.Services.Interfaces;
using DTA.Models.Options;
using DTA.Models.Protos.Files;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;

namespace DTA.BPS.Services;

/// <summary>
/// Represents a processing service
/// </summary>
public class ProcessingService(IOptions<ServiceConnectionOptions> options) : IProcessingService
{
    /// <inheritdoc />
    public void GetDataAndProcess(int fileId)
    {
        using var channel = GrpcChannel.ForAddress(options.Value.FusHttp2);
        var client = new FileServer.FileServerClient(channel);

        client.GetFile(new FileRequest { Id = fileId });

        Fibonacci(43); // Do some heavy processing
        // Thread.Sleep(3000);
    }

    /// <inheritdoc />
    public long Fibonacci(int n)
    {
        if (n <= 1)
            return n;

        return Fibonacci(n - 1) + Fibonacci(n - 2);
    }

    /// <inheritdoc />
    public List<long> PrimeFactors(long n)
    {
        var factors = new List<long>();

        while (n % 2 == 0)
        {
            factors.Add(2);
            n /= 2;
        }

        for (long i = 3; i <= Math.Sqrt(n); i += 2)
        {
            while (n % i == 0)
            {
                factors.Add(i);
                n /= i;
            }
        }

        if (n > 2)
            factors.Add(n);

        return factors;
    }
}