namespace Libertas.Discord.Adventure.Core.GameModels;

/// <summary>
/// Lightweight value type representing a power level for stats (attack/magic/etc.).
/// </summary>
public record struct PowerLevel
{
    /// <summary>
    /// Minimum allowed level (inclusive).
    /// </summary>
    public const int MinLevel = 0;

    /// <summary>
    /// Numeric value of the power level.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Creates a new <see cref="PowerLevel"/> ensuring the value is not below <see cref="MinLevel"/>.
    /// </summary>
    /// <param name="value">Numeric power value.</param>
    public PowerLevel(int value)
    {
        Value = value switch
        {
            < MinLevel => throw new ArgumentOutOfRangeException(nameof(value), value, $"PowerLevel cannot be less than {MinLevel}."),
            _ => value
        };
    }

    /// <summary>
    /// Determines whether one <see cref="PowerLevel"/> is less than another.
    /// </summary>
    public static bool operator <(PowerLevel left, PowerLevel right) => left.Value < right.Value;

    /// <summary>
    /// Determines whether one <see cref="PowerLevel"/> is greater than another.
    /// </summary>
    public static bool operator >(PowerLevel left, PowerLevel right) => left.Value > right.Value;

    /// <summary>
    /// Determines whether one <see cref="PowerLevel"/> is less than or equal to another.
    /// </summary>
    public static bool operator <=(PowerLevel left, PowerLevel right) => left.Value <= right.Value;

    /// <summary>
    /// Determines whether one <see cref="PowerLevel"/> is greater than or equal to another.
    /// </summary>
    public static bool operator >=(PowerLevel left, PowerLevel right) => left.Value >= right.Value;

    /// <summary>
    /// Returns the numeric representation as a string.
    /// </summary>
    public override readonly string ToString() => Value.ToString();
}
