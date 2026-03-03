using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Combat;

/// <summary>
/// Default implementation of <see cref="IDamageApplier"/>.
/// Applies damage to players and mobs, and awards defense XP via the progression service.
/// </summary>
/// <remarks>
/// Constructs the damage applier with the required progression service.
/// </remarks>
/// <param name="progression">Service for calculating defense XP.</param>
public class DamageApplier(IPlayerProgressionService progression) : IDamageApplier
{
    private readonly IPlayerProgressionService _progression = progression ?? throw new ArgumentNullException(nameof(progression));

    /// <summary>
    /// Applies damage to a player, updates HP, and awards defense XP if applicable.
    /// </summary>
    /// <param name="target">Player to apply damage to.</param>
    /// <param name="rawDamage">Raw incoming damage.</param>
    /// <param name="context">Optional combat context.</param>
    /// <returns>Tuple of reduced damage and amount blocked.</returns>
    public (int Reduced, int Blocked) ApplyDamageToPlayer(PlayerState target, int rawDamage, CombatContext<PlayerState>? context = null)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (rawDamage <= 0)
        {
            return (0, 0);
        }

        // Calculate blocked and reduced damage based on defense
        var blocked = Math.Min(Math.Max(0, rawDamage - 1), target.DefensePower.Value);
        var reduced = Math.Max(1, rawDamage - target.DefensePower.Value);

        // Apply reduced damage to player HP
        target.CurrentHp = Math.Max(0, target.CurrentHp - reduced);

        // Award defense XP for humans only
        if (!target.IsBot)
        {
            var defenseXp = _progression.CalculateDefenseXpFromDamage(reduced);
            target.DefenseXpEarned += defenseXp;
        }

        return (reduced, blocked);
    }

    /// <summary>
    /// Applies damage to a mob, updating its HP.
    /// </summary>
    /// <param name="mob">Mob to apply damage to.</param>
    /// <param name="damage">Amount of damage to apply.</param>
    /// <param name="context">Optional combat context.</param>
    /// <returns>Actual damage applied (cannot exceed mob's current HP).</returns>
    public int ApplyDamageToMob(MobState mob, int damage, CombatContext<PlayerState>? context = null)
    {
        ArgumentNullException.ThrowIfNull(mob);

        if (damage <= 0)
        {
            return 0;
        }

        var actual = Math.Min(damage, mob.CurrentHp);
        mob.CurrentHp = Math.Max(0, mob.CurrentHp - actual);
        return actual;
    }
}
