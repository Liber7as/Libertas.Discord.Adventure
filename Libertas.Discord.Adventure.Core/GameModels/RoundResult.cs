namespace Libertas.Discord.Adventure.Core.GameModels;

/// <summary>
/// Result of executing a single combat round.
/// Contains the updated state of all combatants and a log of events.
/// </summary>
/// <remarks>
/// The collections are read-only views of the state after the round completed.
/// Player and mob states reflect any damage taken, XP earned, or deaths that occurred.
/// </remarks>
public record RoundResult
{
    /// <summary>
    /// The dungeon level at which this round was executed.
    /// </summary>
    public int Level { get; init; }

    /// <summary>
    /// All players after the round (may include dead players and newly added bots).
    /// </summary>
    public IReadOnlyList<PlayerState> Players { get; init; } = [];

    /// <summary>
    /// All mobs after the round (may include dead mobs).
    /// </summary>
    public IReadOnlyList<MobState> Mobs { get; init; } = [];

    /// <summary>
    /// Chronological log of combat events and messages generated during the round.
    /// Suitable for display to users.
    /// </summary>
    public IReadOnlyList<string> Messages { get; init; } = [];
}
