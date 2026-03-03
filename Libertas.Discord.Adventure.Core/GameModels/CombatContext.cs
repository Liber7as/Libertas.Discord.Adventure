namespace Libertas.Discord.Adventure.Core.GameModels;

/// <summary>
/// Provides context for combat action resolution.
/// Contains the current actor, all combatants, and the message log for the round.
/// </summary>
/// <typeparam name="TActor">
/// The type of the current actor (<see cref="PlayerState"/> or <see cref="MobState"/>).
/// </typeparam>
/// <param name="Level">Current dungeon level (affects damage scaling and XP rewards).</param>
/// <param name="Actor">The entity currently taking action.</param>
/// <param name="Players">All players in combat (mutable - may be modified during action resolution).</param>
/// <param name="Mobs">All mobs in combat (mutable - may be modified during action resolution).</param>
/// <param name="Messages">Combat log messages generated during this round (append-only).</param>
public record CombatContext<TActor>(
    int Level,
    TActor Actor,
    List<PlayerState> Players,
    List<MobState> Mobs,
    List<string> Messages);
