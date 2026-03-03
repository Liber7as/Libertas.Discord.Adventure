namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
/// Service for calculating player stats based on individual skill levels.
/// Handles skill-based progression where each skill levels up independently.
/// </summary>
public interface IPlayerProgressionService
{
    /// <summary>
    /// Calculates the combat stats for a player based on their skill levels.
    /// </summary>
    PlayerStats CalculateStats(SkillLevels skillLevels);

    /// <summary>
    /// Calculates the total XP required to reach the specified skill level.
    /// </summary>
    /// <param name="level">The target skill level.</param>
    /// <returns>Total XP required from level 1 to reach this level.</returns>
    long GetXpRequiredForSkillLevel(int level);

    /// <summary>
    /// Calculates the XP needed to advance from the current level to the next.
    /// </summary>
    /// <param name="currentLevel">The current skill level.</param>
    /// <returns>XP required to reach the next level.</returns>
    long GetXpToNextSkillLevel(int currentLevel);

    /// <summary>
    /// Determines the skill level for a given total XP.
    /// </summary>
    /// <param name="totalXp">The total accumulated XP for this skill.</param>
    /// <returns>The skill level.</returns>
    int GetSkillLevelForXp(long totalXp);

    /// <summary>
    /// Calculates XP reward for using a skill successfully.
    /// </summary>
    /// <param name="dungeonLevel">The current dungeon level.</param>
    /// <returns>XP to award for the skill.</returns>
    int CalculateSkillXp(int dungeonLevel);

    /// <summary>
    /// Calculates Defense XP for taking damage.
    /// </summary>
    /// <param name="damageTaken">Amount of damage taken.</param>
    /// <returns>Defense XP to award.</returns>
    int CalculateDefenseXpFromDamage(int damageTaken);
}