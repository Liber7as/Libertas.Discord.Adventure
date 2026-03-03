using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Combat;

/// <summary>
///     Responsible for awarding skill experience (XP) to players.
///     Extracted from action handlers to centralize XP calculation and related rules.
/// </summary>
public interface IXpDistributor
{
    /// <summary>
    ///     Awards XP for a specific skill to the provided player for the given dungeon level.
    ///     Implementations should avoid awarding XP to bots when appropriate.
    /// </summary>
    /// <param name="player">The player receiving XP.</param>
    /// <param name="skill">The skill that should receive XP.</param>
    /// <param name="dungeonLevel">Current dungeon level used to scale XP.</param>
    void AwardSkillXp(PlayerState player, SkillType skill, int dungeonLevel);
}