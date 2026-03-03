namespace Libertas.Discord.Adventure.Core.Settings;

/// <summary>
///     Configuration settings for skill-based player progression.
///     Players level up individual skills by using them in combat.
/// </summary>
public class ProgressionSettings
{
    #region Base Stats (All Skills at Level 1)

    /// <summary>
    ///     Base HP before Defense level scaling.
    /// </summary>
    public int BaseHp { get; set; } = 20;

    /// <summary>
    ///     Base attack power at Attack level 1.
    /// </summary>
    public int BaseAttackPower { get; set; } = 5;

    /// <summary>
    ///     Base magic power at Magic level 1.
    /// </summary>
    public int BaseMagicPower { get; set; } = 5;

    /// <summary>
    ///     Base speech power at Speech level 1.
    /// </summary>
    public int BaseSpeechPower { get; set; } = 5;

    /// <summary>
    ///     Base defense power at Defense level 1.
    /// </summary>
    public int BaseDefensePower { get; set; } = 2;

    #endregion

    #region Per-Level Scaling

    /// <summary>
    ///     HP gained per Defense level.
    ///     HP = BaseHp + (DefenseLevel - 1) * HpPerDefenseLevel
    /// </summary>
    public int HpPerDefenseLevel { get; set; } = 5;

    /// <summary>
    ///     Attack power gained per Attack level.
    /// </summary>
    public int AttackPerLevel { get; set; } = 2;

    /// <summary>
    ///     Magic power gained per Magic level.
    /// </summary>
    public int MagicPerLevel { get; set; } = 2;

    /// <summary>
    ///     Speech power gained per Speech level.
    /// </summary>
    public int SpeechPerLevel { get; set; } = 2;

    /// <summary>
    ///     Defense power gained per Defense level.
    /// </summary>
    public int DefensePerLevel { get; set; } = 1;

    #endregion

    #region Skill XP

    /// <summary>
    ///     Base XP awarded for using a skill successfully.
    /// </summary>
    public int BaseSkillXp { get; set; } = 10;

    /// <summary>
    ///     Additional XP per dungeon level when using a skill.
    /// </summary>
    public int SkillXpPerDungeonLevel { get; set; } = 2;

    /// <summary>
    ///     XP multiplier for level requirement.
    ///     Formula: XpToLevel = level * XpPerLevelMultiplier
    ///     Skill Level 2 = 50 XP, Level 3 = 100 XP, etc.
    /// </summary>
    public int SkillXpPerLevel { get; set; } = 50;

    /// <summary>
    ///     Maximum level for any individual skill.
    /// </summary>
    public int MaxSkillLevel { get; set; } = 99;

    /// <summary>
    ///     XP awarded for taking damage (Defense skill).
    ///     Scales with damage taken.
    /// </summary>
    public double DefenseXpPerDamage { get; set; } = 0.5;

    #endregion
}