using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Discord.Services;

/// <summary>
/// Represents an active adventure session in a Discord channel.
/// Contains all mutable state for an ongoing combat encounter.
/// </summary>
/// <remarks>
/// <para>Sessions are managed per-channel and persist across multiple combat rounds.</para>
/// <para>Access to session state should be synchronized using <see cref="SemaphoreSlim"/>
/// or the helper extension methods defined in <see cref="AdventureSessionExtensions"/>.</para>
/// </remarks>
public record AdventureSession
{
    /// <summary>
    /// The Discord channel ID where this session is active.
    /// Used as the primary key for session lookup.
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Current dungeon level (difficulty scaling).
    /// Increases after defeating all mobs in the current wave.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// All players in the session, including human players and AI bots.
    /// Modified during combat as players join, die, or flee.
    /// </summary>
    public List<PlayerState> Players { get; set; } = new List<PlayerState>();

    /// <summary>
    /// All mobs in the current combat encounter.
    /// Cleared and repopulated when advancing to a new level.
    /// </summary>
    public List<MobState> Mobs { get; set; } = new List<MobState>();

    /// <summary>
    /// Actions submitted by players for the upcoming round.
    /// Keyed by player ID. Cleared after each round resolves.
    /// </summary>
    public Dictionary<PlayerId, PlayerAction> PendingActions { get; set; } = new Dictionary<PlayerId, PlayerAction>();

    /// <summary>
    /// Timestamp of the last activity in this session.
    /// Used for timeout detection and cleanup.
    /// </summary>
    public DateTimeOffset LastActivity { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Cancellation token source for the current round timer.
    /// Cancelled when the round ends or session is terminated.
    /// </summary>
    public CancellationTokenSource? RoundCts { get; set; }

    /// <summary>
    /// Semaphore for thread-safe access to session state.
    /// Ensures only one operation modifies the session at a time.
    /// </summary>
    public SemaphoreSlim Semaphore { get; } = new(1, 1);

    /// <summary>
    /// Discord message ID of the current combat status message.
    /// Updated each round to show current action counts and party status.
    /// </summary>
    public ulong? CombatMessageId { get; set; }
}