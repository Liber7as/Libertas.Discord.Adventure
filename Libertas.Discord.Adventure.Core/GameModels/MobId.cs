namespace Libertas.Discord.Adventure.Core.GameModels;

/// <summary>
///     Strongly-typed identifier for a mob instance or preset.
///     Using a distinct type improves clarity and prevents mixing IDs with plain integers.
/// </summary>
public readonly record struct MobId(int Value)
{
    /// <summary>
    ///     Returns the numeric value of the mob id as a string.
    /// </summary>
    public override string ToString()
    {
        return Value.ToString();
    }
}