using Libertas.Discord.Adventure.Core.Settings;
using Microsoft.Extensions.Options;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
///     Default implementation of <see cref="IPlayerProgressionService" />.
/// </summary>
public class PlayerProgressionService(IOptions<ProgressionSettings> options) : IPlayerProgressionService
{
    private readonly ProgressionSettings _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public PlayerStats CalculateStats(SkillLevels skillLevels)
    {
        // Calculate each stat based on its skill level
        var hp = _settings.BaseHp + (skillLevels.DefenseLevel - 1) * _settings.HpPerDefenseLevel;
        var attack = _settings.BaseAttackPower + (skillLevels.AttackLevel - 1) * _settings.AttackPerLevel;
        var magic = _settings.BaseMagicPower + (skillLevels.MagicLevel - 1) * _settings.MagicPerLevel;
        var speech = _settings.BaseSpeechPower + (skillLevels.SpeechLevel - 1) * _settings.SpeechPerLevel;
        var defense = _settings.BaseDefensePower + (skillLevels.DefenseLevel - 1) * _settings.DefensePerLevel;

        // Ensure minimums
        return new PlayerStats(
            Math.Max(1, hp),
            Math.Max(1, attack),
            Math.Max(1, magic),
            Math.Max(1, speech),
            Math.Max(0, defense)
        );
    }

    /// <inheritdoc />
    public long GetXpRequiredForSkillLevel(int level)
    {
        if (level <= 1)
        {
            return 0;
        }

        // Sum of XP for each level: 50 + 100 + 150 + ... + (level-1)*50
        // = 50 * (1 + 2 + 3 + ... + (level-1))
        // = 50 * ((level-1) * level / 2)
        long n = level - 1;
        return _settings.SkillXpPerLevel * (n * (n + 1) / 2);
    }

    /// <inheritdoc />
    public long GetXpToNextSkillLevel(int currentLevel)
    {
        if (currentLevel >= _settings.MaxSkillLevel)
        {
            return 0;
        }

        // XP to reach next level = currentLevel * multiplier
        return currentLevel * _settings.SkillXpPerLevel;
    }

    /// <inheritdoc />
    public int GetSkillLevelForXp(long totalXp)
    {
        var level = 1;
        while (level < _settings.MaxSkillLevel && GetXpRequiredForSkillLevel(level + 1) <= totalXp)
        {
            level++;
        }

        return level;
    }

    /// <inheritdoc />
    public int CalculateSkillXp(int dungeonLevel)
    {
        return _settings.BaseSkillXp + dungeonLevel * _settings.SkillXpPerDungeonLevel;
    }

    /// <inheritdoc />
    public int CalculateDefenseXpFromDamage(int damageTaken)
    {
        return (int)Math.Ceiling(damageTaken * _settings.DefenseXpPerDamage);
    }
}