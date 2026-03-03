using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
///     Service responsible for resolving combat actions during a round.
///     Dispatches player actions to appropriate handlers and processes mob attacks.
/// </summary>
/// <remarks>
///     This is the main entry point for action execution during combat.
///     The implementation uses the Strategy pattern to delegate to specialized handlers.
/// </remarks>
public interface IActionResolutionService
{
    /// <summary>
    ///     Resolves a player's action for the current round.
    ///     Applies damage, healing, XP awards, and loot distribution as appropriate.
    /// </summary>
    /// <param name="context">Combat context containing the actor, all combatants, and message log.</param>
    /// <param name="action">The action the player chose to perform.</param>
    void HandlePlayerAction(CombatContext<PlayerState> context, PlayerAction action);

    /// <summary>
    ///     Resolves a mob's attack action for the current round.
    ///     Mobs attack a player target, potentially killing them.
    /// </summary>
    /// <param name="context">Combat context containing the mob actor, all combatants, and message log.</param>
    void HandleMobAction(CombatContext<MobState> context);
}