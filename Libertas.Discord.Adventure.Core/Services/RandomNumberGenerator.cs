using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
///     Default implementation of <see cref="IRandomNumberGenerator" /> that uses <see cref="System.Random.Shared" />.
/// </summary>
/// <remarks>
///     This implementation delegates to <see cref="System.Random.Shared" />, which is a thread-safe global
///     <see cref="System.Random" /> instance suitable for concurrent use. It avoids explicit locks to provide
///     low-overhead concurrent access. If deterministic sequences are required for testing, consider providing
///     a different implementation of <see cref="IRandomNumberGenerator" /> that accepts a seed.
/// </remarks>
public class RandomNumberGenerator : IRandomNumberGenerator
{
    /// <inheritdoc />
    public int Next(int minValue, int maxValue)
    {
        return Random.Shared.Next(minValue, maxValue);
    }

    /// <inheritdoc />
    public T? GetRandom<T>(List<T> items) where T : class
    {
        if (items.Count == 0)
        {
            return null;
        }

        // Use Random.Next with the list count to avoid modulo bias.
        var index = Next(0, items.Count);

        return items[index];
    }

    /// <inheritdoc />
    public T? GetRandomAlive<T>(List<T> items) where T : class, IAlive
    {
        if (items.Count == 0)
        {
            return null;
        }

        var alive = items.Where(i => i.IsAlive).ToList();

        if (alive.Count == 0)
        {
            return null;
        }

        return GetRandom(alive);
    }
}