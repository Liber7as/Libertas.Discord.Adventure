using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Combat;

/// <summary>
///     Applies damage to a target actor and records defensive XP.
/// </summary>
public interface IDamageApplier
{
    /// <summary>
    ///     Applies damage to the target player and returns a tuple of (reduced damage applied, amount blocked).
    ///     Also awards defense XP to the target as appropriate.
    /// </summary>
    (int Reduced, int Blocked) ApplyDamageToPlayer(PlayerState target, int rawDamage, CombatContext<PlayerState>? context = null);

    /// <summary>
    ///     Applies damage to a mob and returns the actual damage applied.
    /// </summary>
    int ApplyDamageToMob(MobState mob, int damage, CombatContext<PlayerState>? context = null);
}