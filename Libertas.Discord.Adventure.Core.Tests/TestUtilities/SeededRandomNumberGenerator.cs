using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;

namespace Libertas.Discord.Adventure.Core.Tests.TestUtilities;

/// <summary>
///     A seeded random number generator for deterministic test results.
///     Wraps <see cref="Random" /> with a fixed seed.
/// </summary>
public class SeededRandomNumberGenerator(int seed) : IRandomNumberGenerator
{
    private readonly Random _random = new(seed);

    public int Next(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }

    public T? GetRandom<T>(List<T> items) where T : class
    {
        if (items == null || items.Count == 0)
        {
            return null;
        }

        return items[_random.Next(items.Count)];
    }

    public T? GetRandomAlive<T>(List<T> items) where T : class, IAlive
    {
        var alive = items?.Where(x => x.IsAlive).ToList();
        if (alive == null || alive.Count == 0)
        {
            return null;
        }

        return alive[_random.Next(alive.Count)];
    }
}