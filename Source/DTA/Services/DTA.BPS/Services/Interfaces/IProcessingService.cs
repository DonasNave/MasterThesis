namespace DTA.BPS.Services.Interfaces;

public interface IProcessingService
{
    void GetDataAndProcess(int fileId);
    long Fibonacci(int n);
    List<long> PrimeFactors(long n);
}