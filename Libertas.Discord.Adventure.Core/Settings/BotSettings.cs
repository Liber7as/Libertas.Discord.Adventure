namespace Libertas.Discord.Adventure.Core.Settings;

/// <summary>
/// Configuration settings for AI companion bots that fill party slots.
/// </summary>
public class BotSettings
{
    /// <summary>
    /// Minimum number of players (including bots) required for a combat encounter.
    /// Bots will be added to reach this threshold when fewer human players are present.
    /// </summary>
    public int MinimumPartySize { get; set; } = 4;

    /// <summary>
    /// Maximum variance (±) applied to bot stats relative to the average human player level.
    /// For example, if average player level is 5 and variance is 2, bots will have stats scaled to levels 3-7.
    /// </summary>
    public int StatLevelVariance { get; set; } = 2;

    /// <summary>
    /// Base HP value used when scaling bot health.
    /// Actual HP = BaseHp + (level * HpPerLevel).
    /// </summary>
    public int BaseHp { get; set; } = 20;

    /// <summary>
    /// Additional HP gained per level when scaling bot health.
    /// </summary>
    public int HpPerLevel { get; set; } = 5;

    /// <summary>
    /// Base power value for attack, magic, speech, and defense stats.
    /// Actual power = BasePower + (level * PowerPerLevel).
    /// </summary>
    public int BasePower { get; set; } = 1;

    /// <summary>
    /// Additional power gained per level for attack, magic, speech, and defense stats.
    /// </summary>
    public int PowerPerLevel { get; set; } = 2;

    /// <summary>
    /// Minimum level for generated bots (prevents negative or zero levels from variance).
    /// </summary>
    public int MinimumBotLevel { get; set; } = 1;

    /// <summary>
    /// Thematic names for AI companion bots. A random name is selected for each bot.
    /// </summary>
    public List<string> BotNames { get; set; } = [];
}
