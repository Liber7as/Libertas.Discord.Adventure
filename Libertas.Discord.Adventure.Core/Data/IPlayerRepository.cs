using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Data;

/// <summary>
/// Repository interface used by core services to load and persist player state.
/// This abstraction keeps the core logic free of EF/Discord-specific types.
/// </summary>
public interface IPlayerRepository
{
    /// <summary>
    /// Retrieves a <see cref="PlayerState"/> by its identifier or <c>null</c> when not found.
    /// </summary>
    Task<PlayerState?> GetByIdAsync(PlayerId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an existing <see cref="PlayerState"/> or creates a new persistent record when none exists.
    /// The returned <see cref="PlayerState"/> should be tracked for updates by callers.
    /// </summary>
    Task<PlayerState> GetOrCreateAsync(PlayerId id, string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists changes to a player's state.
    /// </summary>
    Task UpdateAsync(PlayerState player, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the top players ordered by total level (sum of all skills).
    /// </summary>
    Task<List<PlayerState>> GetTopPlayersByTotalLevelAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the top players ordered by their highest dungeon reached.
    /// </summary>
    Task<List<PlayerState>> GetTopPlayersByHighestDungeonAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the top players ordered by total gold earned.
    /// </summary>
    Task<List<PlayerState>> GetTopPlayersByGoldAsync(int count = 10, CancellationToken cancellationToken = default);
}
