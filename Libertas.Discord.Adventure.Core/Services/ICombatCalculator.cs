using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
/// Provides combat-related calculations (damage, success chances, loot).
/// Separates game math from action execution for testability and clarity.
/// </summary>
public interface ICombatCalculator
{
    /// <summary>
    /// Calculates physical attack damage with critical hit determination.
    /// </summary>
    /// <param name="attackPower">The attacker's attack power.</param>
    /// <returns>Tuple of (damage dealt, whether it was a critical hit).</returns>
    (int Damage, bool IsCritical) CalculateAttackDamage(int attackPower);

    /// <summary>
    /// Calculates magic damage with critical hit determination.
    /// </summary>
    /// <param name="magicPower">The caster's magic power.</param>
    /// <returns>Tuple of (damage dealt, whether it was a critical hit).</returns>
    (int Damage, bool IsCritical) CalculateMagicDamage(int magicPower);

    /// <summary>
    /// Calculates mob attack damage with level scaling and variance.
    /// </summary>
    /// <param name="mobAttackPower">The mob's base attack power.</param>
    /// <param name="dungeonLevel">Current dungeon level for scaling.</param>
    /// <returns>Raw damage before defense reduction.</returns>
    int CalculateMobDamage(int mobAttackPower, int dungeonLevel);

    /// <summary>
    /// Applies defense reduction to incoming damage.
    /// </summary>
    /// <param name="rawDamage">Damage before reduction.</param>
    /// <param name="defensePower">Target's defense power.</param>
    /// <returns>Tuple of (reduced damage, amount blocked). Minimum 1 damage.</returns>
    (int ReducedDamage, int AmountBlocked) ApplyDefenseReduction(int rawDamage, int defensePower);

    /// <summary>
    /// Calculates the success chance for the Talk action (diplomacy).
    /// </summary>
    /// <param name="speechPower">The speaker's speech power.</param>
    /// <param name="mobAttackPower">The target mob's attack power (represents stubbornness).</param>
    /// <param name="dungeonLevel">Current dungeon level.</param>
    /// <returns>Success chance as percentage (0-100).</returns>
    int CalculateTalkSuccessChance(int speechPower, int mobAttackPower, int dungeonLevel);

    /// <summary>
    /// Calculates the success chance for Prayer's divine smite.
    /// </summary>
    /// <param name="dungeonLevel">Current dungeon level.</param>
    /// <returns>Success chance as percentage (0-100).</returns>
    int CalculatePraySmiteChance(int dungeonLevel);

    /// <summary>
    /// Calculates self-heal amount when prayer fails.
    /// </summary>
    /// <param name="maxHp">The player's maximum HP.</param>
    /// <returns>Amount of HP to restore.</returns>
    int CalculatePrayHealAmount(int maxHp);

    /// <summary>
    /// Calculates heal amount for the Heal action.
    /// </summary>
    /// <param name="magicPower">The healer's magic power.</param>
    /// <returns>Amount of HP to restore.</returns>
    int CalculateHealAmount(int magicPower);

    /// <summary>
    /// Calculates escape chance for the Run action.
    /// </summary>
    /// <param name="currentHp">Runner's current HP.</param>
    /// <param name="maxHp">Runner's maximum HP.</param>
    /// <param name="playerCount">Number of players in combat.</param>
    /// <param name="mobCount">Number of mobs in combat.</param>
    /// <returns>Success chance as percentage (0-100).</returns>
    int CalculateRunChance(int currentHp, int maxHp, int playerCount, int mobCount);

    /// <summary>
    /// Calculates gold dropped by a defeated mob.
    /// </summary>
    /// <param name="mobAttackPower">The mob's attack power (stronger = more gold).</param>
    /// <param name="dungeonLevel">Current dungeon level.</param>
    /// <returns>Gold amount to distribute.</returns>
    int CalculateLootGold(int mobAttackPower, int dungeonLevel);

    /// <summary>
    /// Rolls a percentage check (0-100).
    /// </summary>
    /// <param name="successChance">Required roll to succeed (0-100).</param>
    /// <returns>True if the roll succeeded.</returns>
    bool RollChance(int successChance);

    /// <summary>
    /// Selects a random alive target from a list.
    /// </summary>
    T? SelectRandomTarget<T>(IEnumerable<T> targets) where T : class, IAlive;

    /// <summary>
    /// Selects a mob target with preference for lowest HP player.
    /// </summary>
    /// <param name="players">Available player targets.</param>
    /// <returns>Selected target or null if none alive.</returns>
    PlayerState? SelectMobTarget(IReadOnlyList<PlayerState> players);
}
