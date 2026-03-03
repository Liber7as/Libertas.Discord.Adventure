using Libertas.Discord.Adventure.Core.Services;

namespace Libertas.Discord.Adventure.Core.GameModels;

/// <summary>
///     Represents a player's state during combat.
///     This is a mutable class that changes during combat rounds.
///     Combat stats are calculated from individual skill levels.
/// </summary>
public class PlayerState : IAlive
{
    /// <summary>
    ///     Unique identifier for the player (Discord user ID for humans, generated for bots).
    /// </summary>
    public required PlayerId Id { get; set; }

    /// <summary>
    ///     Display name of the player.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    #region Skill Levels (From Persistent Data)

    /// <summary>
    ///     The player's individual skill levels.
    ///     Skills level up by using them in combat.
    /// </summary>
    public SkillLevels Skills { get; set; } = SkillLevels.Default;

    #endregion

    /// <summary>
    ///     Timestamps persisted for the player record.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Timestamp of the player's last activity (used for cleanup/idle detection).
    /// </summary>
    public DateTimeOffset LastActiveAt { get; set; }

    #region Combat Stats (Calculated from Skill Levels)

    /// <summary>
    ///     Maximum hit points for the player (derived from Defense skill and progression).
    /// </summary>
    public int MaxHp { get; set; } = 20;

    /// <summary>
    ///     Current hit points during combat.
    /// </summary>
    public int CurrentHp { get; set; } = 20;

    /// <summary>
    ///     Offensive power derived from Attack skill level.
    /// </summary>
    public PowerLevel AttackPower { get; set; } = new(1);

    /// <summary>
    ///     Magical power derived from Magic skill level.
    /// </summary>
    public PowerLevel MagicPower { get; set; } = new(1);

    /// <summary>
    ///     Speech power derived from Speech skill level (affects Talk actions).
    /// </summary>
    public PowerLevel SpeechPower { get; set; } = new(1);

    /// <summary>
    ///     Defensive power derived from Defense skill level (reduces incoming damage).
    /// </summary>
    public PowerLevel DefensePower { get; set; } = new(1);

    #endregion

    #region Session Earnings (Accumulated During Combat)

    /// <summary>
    ///     Gold earned during this session.
    ///     Will be added to persistent total after session ends.
    /// </summary>
    public double GoldEarned { get; set; } = 0.00;

    /// <summary>
    ///     Number of mobs killed during this session.
    ///     Will be added to persistent total after session ends.
    /// </summary>
    public int MobsKilled { get; set; } = 0;

    /// <summary>
    ///     Attack XP earned during this session.
    /// </summary>
    public long AttackXpEarned { get; set; } = 0;

    /// <summary>
    ///     Magic XP earned during this session.
    /// </summary>
    public long MagicXpEarned { get; set; } = 0;

    /// <summary>
    ///     Speech XP earned during this session.
    /// </summary>
    public long SpeechXpEarned { get; set; } = 0;

    /// <summary>
    ///     Defense XP earned during this session.
    /// </summary>
    public long DefenseXpEarned { get; set; } = 0;

    #endregion

    #region Persistent Fields (Loaded from DB)

    /// <summary>
    ///     Total Attack XP stored in persistence (lifetime).
    /// </summary>
    public long AttackXpTotal { get; set; }

    /// <summary>
    ///     Total Magic XP stored in persistence (lifetime).
    /// </summary>
    public long MagicXpTotal { get; set; }

    /// <summary>
    ///     Total Speech XP stored in persistence (lifetime).
    /// </summary>
    public long SpeechXpTotal { get; set; }

    /// <summary>
    ///     Total Defense XP stored in persistence (lifetime).
    /// </summary>
    public long DefenseXpTotal { get; set; }

    /// <summary>
    ///     Total gold accumulated across all sessions (persistent).
    /// </summary>
    public double TotalGold { get; set; }

    /// <summary>
    ///     Total mobs killed across all sessions (persistent).
    /// </summary>
    public int TotalKills { get; set; }

    /// <summary>
    ///     Highest dungeon level reached (persistent).
    /// </summary>
    public int HighestDungeonLevel { get; set; }

    /// <summary>
    ///     Total deaths across all sessions.
    /// </summary>
    public int TotalDeaths { get; set; }

    #endregion


    #region Flags

    /// <summary>
    ///     Indicates whether this player is an AI companion bot.
    ///     Bots participate in combat but do not receive loot or XP rewards.
    /// </summary>
    public bool IsBot { get; init; }

    /// <summary>
    ///     Whether the player is currently alive (HP > 0).
    /// </summary>
    public bool IsAlive => CurrentHp > 0;

    #endregion

    #region Computed Properties

    /// <summary>
    ///     Combined "combat level" for display (average of all skills).
    /// </summary>
    public int CombatLevel => Skills.CombatLevel;

    /// <summary>
    ///     Total of all skill levels.
    /// </summary>
    public int TotalLevel => Skills.TotalLevel;

    #endregion
}