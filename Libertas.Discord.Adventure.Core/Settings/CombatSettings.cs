namespace Libertas.Discord.Adventure.Core.Settings;

/// <summary>
/// Configuration settings for combat mechanics.
/// All values can be tuned via appsettings.json under the "Combat" section.
/// </summary>
/// <remarks>
/// <para><b>Percentage Convention:</b> All chances/percentages use integers 0-100 (not 0.0-1.0).</para>
/// <para><b>Multiplier Convention:</b> All multipliers use doubles (e.g., 2.0 = double damage).</para>
/// </remarks>
public class CombatSettings
{
    #region Critical Hits

    /// <summary>
    /// Chance (0-100) for an attack or magic spell to critically hit.
    /// Default: 15 (15% chance).
    /// </summary>
    /// <example>15 means 15% crit chance.</example>
    public int CriticalHitChance { get; set; } = 15;

    /// <summary>
    /// Damage multiplier applied on critical hits.
    /// Default: 2.0 (double damage).
    /// </summary>
    /// <example>2.0 means crits deal 2x normal damage.</example>
    public double CriticalHitMultiplier { get; set; } = 2.0;

    #endregion

    #region Mob Behavior

    /// <summary>
    /// Chance (0-100) that a mob will target the player with the lowest HP.
    /// If this check fails, a random alive player is targeted instead.
    /// Default: 70 (70% chance to focus low HP targets).
    /// </summary>
    public int MobTargetLowestHpChance { get; set; } = 70;

    /// <summary>
    /// Base multiplier for mob damage scaling per dungeon level.
    /// Mob damage = BaseDamage × (MobDamageScalingMultiplier ^ Level).
    /// Default: 1.05 (5% increase per level, exponential).
    /// </summary>
    /// <example>At level 10: damage × 1.05^10 ? 1.63x damage.</example>
    public double MobDamageScalingMultiplier { get; set; } = 1.05;

    /// <summary>
    /// Divisor used to calculate damage variance range.
    /// Variance = BaseDamage / DamageVarianceDivisor.
    /// Actual damage = BaseDamage ± Variance.
    /// Default: 4 (±25% variance).
    /// </summary>
    /// <example>With 20 base damage and divisor 4: damage ranges 15-25.</example>
    public int DamageVarianceDivisor { get; set; } = 4;

    #endregion

    #region Talk (Diplomacy)

    /// <summary>
    /// Multiplier applied to SpeechPower when calculating talk success.
    /// Formula: SuccessChance = (SpeechPower × Multiplier) / (MobPower + Level/2).
    /// Default: 1.0 (no modification).
    /// </summary>
    public double TalkPowerMultiplier { get; set; } = 1.0;

    /// <summary>
    /// Divisor for dungeon level contribution to talk difficulty.
    /// Higher values make talk easier at high levels.
    /// Formula contribution: Level / TalkLevelDivisor.
    /// Default: 2 (level 10 adds 5 to difficulty).
    /// </summary>
    public int TalkLevelDivisor { get; set; } = 2;

    #endregion

    #region Prayer (Divine Smite / Self-Heal)

    /// <summary>
    /// Base chance (0-100) for prayer to instantly kill a mob (Divine Smite).
    /// Default: 10 (10% base chance).
    /// </summary>
    public int PraySmiteBaseChance { get; set; } = 10;

    /// <summary>
    /// Level at which prayer success chance starts decreasing.
    /// Formula: SuccessChance = BaseChance + (ThresholdLevel - CurrentLevel).
    /// Default: 20 (at level 20, only base chance remains).
    /// </summary>
    /// <example>At level 5: 10 + (20 - 5) = 25% success chance.</example>
    public int PraySmiteLevelThreshold { get; set; } = 20;

    /// <summary>
    /// Minimum chance (0-100) for prayer to succeed, regardless of level.
    /// Default: 5 (always at least 5% chance).
    /// </summary>
    public int PraySmiteMinChance { get; set; } = 5;

    /// <summary>
    /// Maximum chance (0-100) for prayer to succeed, regardless of level.
    /// Default: 30 (capped at 30% at low levels).
    /// </summary>
    public int PraySmiteMaxChance { get; set; } = 30;

    /// <summary>
    /// Maximum percentage (0-100) of MaxHP that prayer self-heal can restore.
    /// Actual heal is random from 1 to this percentage of MaxHP.
    /// Default: 25 (up to 25% of max HP).
    /// </summary>
    /// <example>Player with 100 MaxHP heals 1-25 HP on failed prayer.</example>
    public int PrayHealMaxPercent { get; set; } = 25;

    #endregion

    #region Healing

    /// <summary>
    /// Divisor applied to MagicPower when calculating heal amount.
    /// Formula: HealAmount = (MagicPower / Divisor) + Random(Min, Max).
    /// Default: 2 (half of magic power contributes to healing).
    /// </summary>
    public int HealPowerDivisor { get; set; } = 2;

    /// <summary>
    /// Minimum random bonus added to heal amount.
    /// Default: 5.
    /// </summary>
    public int HealRandomBonusMin { get; set; } = 5;

    /// <summary>
    /// Maximum random bonus added to heal amount (exclusive).
    /// Default: 11 (actual max is 10).
    /// </summary>
    /// <remarks>This is exclusive, so 11 means random values 5-10.</remarks>
    public int HealRandomBonusMax { get; set; } = 11;

    #endregion

    #region Running (Escape)

    /// <summary>
    /// Base chance (0-100) to successfully flee from combat.
    /// Default: 40 (40% base escape chance).
    /// </summary>
    public int RunBaseChance { get; set; } = 40;

    /// <summary>
    /// Bonus chance (0-100) added when player HP is below the low HP threshold.
    /// Default: 25 (+25% when badly injured).
    /// </summary>
    public int RunLowHpBonus { get; set; } = 25;

    /// <summary>
    /// Divisor to determine "low HP" threshold.
    /// Low HP = CurrentHP &lt; MaxHP / Divisor.
    /// Default: 3 (below 33% HP is considered low).
    /// </summary>
    public int RunLowHpThresholdDivisor { get; set; } = 3;

    /// <summary>
    /// Bonus chance (0-100) added when outnumbered by mobs.
    /// Outnumbered = MobCount &gt; PlayerCount.
    /// Default: 15 (+15% when outnumbered).
    /// </summary>
    public int RunOutnumberedBonus { get; set; } = 15;

    /// <summary>
    /// Minimum escape chance (0-100), regardless of penalties.
    /// Default: 10 (always at least 10% chance).
    /// </summary>
    public int RunMinChance { get; set; } = 10;

    /// <summary>
    /// Maximum escape chance (0-100), regardless of bonuses.
    /// Default: 85 (capped at 85% to maintain tension).
    /// </summary>
    public int RunMaxChance { get; set; } = 85;

    #endregion

    #region Loot

    /// <summary>
    /// Base gold amount before level scaling.
    /// Default: 5.
    /// </summary>
    public int LootBaseGold { get; set; } = 5;

    /// <summary>
    /// Divisor applied to mob's AttackPower for gold calculation.
    /// Stronger mobs drop more gold.
    /// Default: 3.
    /// </summary>
    public int LootPowerDivisor { get; set; } = 3;

    /// <summary>
    /// Gold bonus per dungeon level.
    /// Formula contribution: Level × LootLevelMultiplier.
    /// Default: 4 (level 10 adds 40 gold to base).
    /// </summary>
    public int LootLevelMultiplier { get; set; } = 4;

    /// <summary>
    /// Divisor for calculating loot variance (random bonus range).
    /// MaxGold = BaseGold + (BaseGold / VarianceDivisor).
    /// Default: 2 (+50% variance).
    /// </summary>
    public int LootVarianceDivisor { get; set; } = 2;

    #endregion
}
