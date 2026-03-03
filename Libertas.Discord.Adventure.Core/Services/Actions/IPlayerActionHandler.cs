using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Actions;

/// <summary>
///     Handles execution of a specific player action in combat.
///     Each action type (Attack, Magic, etc.) has its own handler implementation.
/// </summary>
public interface IPlayerActionHandler
{
    /// <summary>
    ///     The action type this handler processes.
    /// </summary>
    PlayerAction Action { get; }

    /// <summary>
    ///     Executes the action within the given combat context.
    /// </summary>
    /// <param name="context">The current combat state including actor, targets, and message log.</param>
    void Execute(CombatContext<PlayerState> context);
}