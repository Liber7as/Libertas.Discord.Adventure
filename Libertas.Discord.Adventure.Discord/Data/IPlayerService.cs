using Discord.WebSocket;
using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Data.Entities;

namespace Libertas.Discord.Adventure.Discord.Data;

/// <summary>
/// Service for managing player state during combat sessions.
/// Loads persistent data from the database and calculates combat stats.
/// </summary>
public interface IPlayerService
{
    /// <summary>
    /// Gets or creates a player state for combat using Discord user information.
    /// Loads from database if exists, creates new record if not.
    /// </summary>
    /// <param name="user">The Discord user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PlayerState> GetOrCreateAsync(SocketUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a player state for combat using ID and username.
    /// Loads from database if exists, creates new record if not.
    /// This overload is useful for testing without Discord dependencies.
    /// </summary>
    /// <param name="userId">The player's unique ID (Discord user ID or test ID).</param>
    /// <param name="username">The player's display name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PlayerState> GetOrCreateAsync(ulong userId, string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves session progress back to the database.
    /// Updates XP, gold, kills, deaths, and highest dungeon level.
    /// Returns information about any skill level-ups.
    /// </summary>
    Task<SaveProgressResult> SaveProgressAsync(PlayerState playerState, int dungeonLevel, bool died, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a player's persistent state from the repository.
    /// Returns a <see cref="PlayerState"/> suitable for core usage, or null if not found.
    /// </summary>
    Task<Libertas.Discord.Adventure.Core.GameModels.PlayerState?> GetPlayerDataAsync(ulong playerId, CancellationToken cancellationToken = default);
}