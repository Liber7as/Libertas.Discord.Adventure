namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
/// Represents a player's skill levels.
/// </summary>
public record SkillLevels(
    int AttackLevel,
    int MagicLevel,
    int SpeechLevel,
    int DefenseLevel
)
{
    /// <summary>
    /// Default skill levels for a new player (all level 1).
    /// </summary>
    public static SkillLevels Default => new(1, 1, 1, 1);

    /// <summary>
    /// Calculates the "total level" (sum of all skills) for display.
    /// </summary>
    public int TotalLevel => AttackLevel + MagicLevel + SpeechLevel + DefenseLevel;

    /// <summary>
    /// Calculates the "combat level" - a single number representing overall power.
    /// </summary>
    public int CombatLevel => (int)Math.Round(TotalLevel / 4.0);
}