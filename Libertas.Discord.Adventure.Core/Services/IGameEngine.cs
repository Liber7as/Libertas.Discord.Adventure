using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
///     Core game engine that orchestrates combat rounds.
///     Coordinates player actions, bot AI, mob attacks, and round summaries.
/// </summary>
/// <remarks>
///     <para>The engine is stateless - all state is passed in and returned via <see cref="RoundResult" />.</para>
///     <para>Round execution order: Bot Injection ? Player Phase ? Mob Phase ? Round Summary.</para>
/// </remarks>
public interface IGameEngine
{
    /// <summary>
    ///     Executes a single combat round, processing all player and mob actions.
    /// </summary>
    /// <param name="level">Current dungeon level (affects scaling and XP rewards).</param>
    /// <param name="players">Current player states. Will be copied internally to avoid mutation.</param>
    /// <param name="playerActions">Actions chosen by players. Players without actions default to Run.</param>
    /// <param name="mobs">Current mobs in combat. Will be copied internally to avoid mutation.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>
    ///     A <see cref="RoundResult" /> containing the updated player and mob states,
    ///     plus a log of all combat messages generated during the round.
    /// </returns>
    Task<RoundResult> ExecuteRoundAsync(
        int level,
        IEnumerable<PlayerState> players,
        IDictionary<PlayerId, PlayerAction> playerActions,
        IEnumerable<MobState> mobs,
        CancellationToken cancellationToken = default);
}