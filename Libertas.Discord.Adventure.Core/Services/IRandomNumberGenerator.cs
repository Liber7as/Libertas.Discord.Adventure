using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
///     Provides random number generation and helpers for selecting random items from collections.
/// </summary>
public interface IRandomNumberGenerator
{
    /// <summary>
    ///     Returns a random integer greater than or equal to <paramref name="minValue" /> and less than
    ///     <paramref name="maxValue" />.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
    /// <returns>
    ///     A 32-bit signed integer greater than or equal to <paramref name="minValue" /> and less than
    ///     <paramref name="maxValue" />.
    /// </returns>
    int Next(int minValue, int maxValue);

    /// <summary>
    ///     Returns a random item from the provided list.
    /// </summary>
    /// <typeparam name="T">The element type of the list. Must be a reference type.</typeparam>
    /// <param name="items">The list to select a random item from.</param>
    /// <returns>
    ///     A random item from <paramref name="items" />, or <c>null</c> if <paramref name="items" /> is <c>null</c> or empty.
    /// </returns>
    T? GetRandom<T>(List<T> items) where T : class;

    /// <summary>
    ///     Returns a random alive item from the provided list. The element type must implement <see cref="IAlive" />.
    /// </summary>
    /// <typeparam name="T">The element type of the list that implements <see cref="IAlive" />.</typeparam>
    /// <param name="items">The list to select a random alive item from.</param>
    /// <returns>
    ///     A random alive item from <paramref name="items" />, or <c>null</c> if <paramref name="items" /> is <c>null</c> or
    ///     no items are alive.
    /// </returns>
    T? GetRandomAlive<T>(List<T> items) where T : class, IAlive;
}