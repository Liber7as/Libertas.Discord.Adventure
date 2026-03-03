namespace Libertas.Discord.Adventure.Core.GameModels;

/// <summary>
///     Skill types that can be leveled by players.
///     Shared public enum used across handlers and services.
/// </summary>
public enum SkillType
{
    /// <summary>
    ///     Attack skill (physical damage).
    /// </summary>
    Attack,

    /// <summary>
    ///     Magic skill (spells/healing).
    /// </summary>
    Magic,

    /// <summary>
    ///     Speech skill (diplomacy/talk actions).
    /// </summary>
    Speech,

    /// <summary>
    ///     Defense skill (damage mitigation and HP scaling).
    /// </summary>
    Defense
}