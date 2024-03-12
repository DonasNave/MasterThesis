using Shared.APP.Models.Signal;

namespace SRS.Helpers;

public static class SignalGenerator
{
    private static readonly Random Random = new();

    public static async Task<List<Signal>> GenerateRandomSignalsAsync(int numberOfSignals)
    {
        var signals = new List<Signal>();

        // Simulate baseline delay
        await Task.Delay(500);

        for (var i = 0; i < numberOfSignals; i++)
        {
            // Simulate a delay per signal
            await Task.Delay(Random.Next(1, 20));

            var signal = new Signal
            {
                Value = (float)(Random.NextDouble() * 100),
                Time = DateTime.Now,
                Unit = GetRandomUnit(),
                Tags = GetRandomTags()
            };

            signal.Value += 10;

            // Simulate a more complex operation
            for (var j = 0; j < Random.Next(5, 20); j++)
            {
                signal.Value *= 1.1f;
            }

            signals.Add(signal);
        }

        return signals;
    }

    private static UnitEnum GetRandomUnit()
    {
        var values = Enum.GetValues(typeof(UnitEnum));
        return (UnitEnum)(values.GetValue(Random.Next(values.Length)) ?? 0);
    }

    private static string[] GetRandomTags()
    {
        var numberOfTags = Random.Next(1, 5);
        var tags = new string[numberOfTags];

        for (var i = 0; i < numberOfTags; i++)
        {
            tags[i] = $"Tag{i + 1}";
        }

        return tags;
    }
}