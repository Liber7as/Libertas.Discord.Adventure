namespace Libertas.Discord.Adventure.Data.Entities;

/// <summary>
///     Persistent player data stored in the database.
///     Tracks individual skill progression and lifetime statistics.
///     Note: Does not implement IEntity because Id is ulong (Discord user ID), not int.
/// </summary>
public class Player
{
    /// <summary>
    ///     Unique identifier for the player (Discord user ID).
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    ///     Display name of the player (updated from Discord on each login).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    #region Skill Levels & XP

    /// <summary>
    ///     Attack skill level (increases by attacking).
    /// </summary>
    public int AttackLevel { get; set; } = 1;

    /// <summary>
    ///     Total Attack XP accumulated.
    /// </summary>
    public long AttackXp { get; set; } = 0;

    /// <summary>
    ///     Magic skill level (increases by casting magic or healing).
    /// </summary>
    public int MagicLevel { get; set; } = 1;

    /// <summary>
    ///     Total Magic XP accumulated.
    /// </summary>
    public long MagicXp { get; set; } = 0;

    /// <summary>
    ///     Speech skill level (increases by talking/diplomacy).
    /// </summary>
    public int SpeechLevel { get; set; } = 1;

    /// <summary>
    ///     Total Speech XP accumulated.
    /// </summary>
    public long SpeechXp { get; set; } = 0;

    /// <summary>
    ///     Defense skill level (increases by taking damage).
    /// </summary>
    public int DefenseLevel { get; set; } = 1;

    /// <summary>
    ///     Total Defense XP accumulated.
    /// </summary>
    public long DefenseXp { get; set; } = 0;

    #endregion

    #region Lifetime Statistics

    /// <summary>
    ///     Total gold accumulated across all sessions.
    /// </summary>
    public double TotalGold { get; set; } = 0.0;

    /// <summary>
    ///     Total number of mobs killed across all sessions.
    /// </summary>
    public int TotalKills { get; set; } = 0;

    /// <summary>
    ///     Total number of deaths across all sessions.
    /// </summary>
    public int TotalDeaths { get; set; } = 0;

    /// <summary>
    ///     Highest dungeon level reached in any session.
    /// </summary>
    public int HighestDungeonLevel { get; set; } = 0;

    #endregion

    #region Timestamps

    /// <summary>
    ///     When the player first joined.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    ///     When the player was last active in a session.
    /// </summary>
    public DateTimeOffset LastActiveAt { get; set; } = DateTimeOffset.UtcNow;

    #endregion

    #region Computed Properties

    /// <summary>
    ///     Sum of all skill levels.
    /// </summary>
    public int TotalLevel => AttackLevel + MagicLevel + SpeechLevel + DefenseLevel;

    /// <summary>
    ///     Average skill level (combat level).
    /// </summary>
    public int CombatLevel => (int)Math.Round(TotalLevel / 4.0);

    #endregion
}