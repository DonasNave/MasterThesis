namespace SRS.Services.Interfaces;

public interface IReadingService
{
    Task<IResult> GetRandomSignals(int count);
}