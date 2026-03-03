using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Combat;

/// <summary>
/// Default implementation of <see cref="IXpDistributor"/>.
/// Delegates XP calculations to <see cref="IPlayerProgressionService"/>.
/// </summary>
public class XpDistributor(IPlayerProgressionService progression) : IXpDistributor
{
    private readonly IPlayerProgressionService _progression = progression ?? throw new ArgumentNullException(nameof(progression));

    /// <summary>
    /// Awards skill XP to a player for using a skill at the given dungeon level.
    /// Bots do not receive XP.
    /// </summary>
    /// <param name="player">Target player state.</param>
    /// <param name="skill">Skill type that earned XP.</param>
    /// <param name="dungeonLevel">Current dungeon level used to calculate XP amount.</param>
    public void AwardSkillXp(PlayerState player, SkillType skill, int dungeonLevel)
    {
        ArgumentNullException.ThrowIfNull(player);

        if (player.IsBot)
        {
            return; // bots do not gain XP
        }

        var xp = _progression.CalculateSkillXp(dungeonLevel);

        switch (skill)
        {
            case SkillType.Attack:
                player.AttackXpEarned += xp;
                break;
            case SkillType.Magic:
                player.MagicXpEarned += xp;
                break;
            case SkillType.Speech:
                player.SpeechXpEarned += xp;
                break;
            case SkillType.Defense:
                player.DefenseXpEarned += xp;
                break;
        }
    }
}
