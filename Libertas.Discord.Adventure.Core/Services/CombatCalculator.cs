using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Settings;
using Microsoft.Extensions.Options;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
///     Default implementation of combat calculations using configurable settings.
/// </summary>
public class CombatCalculator(IRandomNumberGenerator rng, IOptions<CombatSettings> options) : ICombatCalculator
{
    private readonly IRandomNumberGenerator _rng = rng ?? throw new ArgumentNullException(nameof(rng));
    private readonly CombatSettings _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public (int Damage, bool IsCritical) CalculateAttackDamage(int attackPower)
    {
        var isCrit = RollChance(_settings.CriticalHitChance);
        var damage = isCrit
            ? (int)(attackPower * _settings.CriticalHitMultiplier)
            : attackPower;
        return (damage, isCrit);
    }

    /// <inheritdoc />
    public (int Damage, bool IsCritical) CalculateMagicDamage(int magicPower)
    {
        var isCrit = RollChance(_settings.CriticalHitChance);
        var damage = isCrit
            ? (int)(magicPower * _settings.CriticalHitMultiplier)
            : magicPower;
        return (damage, isCrit);
    }

    /// <inheritdoc />
    public int CalculateMobDamage(int mobAttackPower, int dungeonLevel)
    {
        var multiplier = Math.Pow(_settings.MobDamageScalingMultiplier, dungeonLevel);
        var baseDamage = (int)Math.Max(1, mobAttackPower * multiplier);
        var variance = Math.Max(1, baseDamage / _settings.DamageVarianceDivisor);
        return _rng.Next(baseDamage - variance, baseDamage + variance + 1);
    }

    /// <inheritdoc />
    public (int ReducedDamage, int AmountBlocked) ApplyDefenseReduction(int rawDamage, int defensePower)
    {
        var blocked = Math.Min(rawDamage - 1, defensePower); // Always at least 1 damage
        var reduced = Math.Max(1, rawDamage - defensePower);
        return (reduced, blocked > 0 ? blocked : 0);
    }

    /// <inheritdoc />
    public int CalculateTalkSuccessChance(int speechPower, int mobAttackPower, int dungeonLevel)
    {
        if (mobAttackPower <= 0)
        {
            return 100; // Passive mobs always persuadable
        }

        var levelContribution = dungeonLevel / (double)_settings.TalkLevelDivisor;
        var difficulty = mobAttackPower + levelContribution;
        var successRate = speechPower * _settings.TalkPowerMultiplier / difficulty;

        return (int)Math.Clamp(successRate * 100, 0, 100);
    }

    /// <inheritdoc />
    public int CalculatePraySmiteChance(int dungeonLevel)
    {
        var chance = _settings.PraySmiteBaseChance + (_settings.PraySmiteLevelThreshold - dungeonLevel);
        return Math.Clamp(chance, _settings.PraySmiteMinChance, _settings.PraySmiteMaxChance);
    }

    /// <inheritdoc />
    public int CalculatePrayHealAmount(int maxHp)
    {
        var maxHeal = Math.Max(1, maxHp * _settings.PrayHealMaxPercent / 100);
        return _rng.Next(1, maxHeal + 1);
    }

    /// <inheritdoc />
    public int CalculateHealAmount(int magicPower)
    {
        var powerContribution = magicPower / _settings.HealPowerDivisor;
        var randomBonus = _rng.Next(_settings.HealRandomBonusMin, _settings.HealRandomBonusMax);
        return powerContribution + randomBonus;
    }

    /// <inheritdoc />
    public int CalculateRunChance(int currentHp, int maxHp, int playerCount, int mobCount)
    {
        var chance = _settings.RunBaseChance;

        // Low HP bonus
        var lowHpThreshold = maxHp / _settings.RunLowHpThresholdDivisor;
        if (currentHp < lowHpThreshold)
        {
            chance += _settings.RunLowHpBonus;
        }

        // Outnumbered bonus
        if (mobCount > playerCount)
        {
            chance += _settings.RunOutnumberedBonus;
        }

        return Math.Clamp(chance, _settings.RunMinChance, _settings.RunMaxChance);
    }

    /// <inheritdoc />
    public int CalculateLootGold(int mobAttackPower, int dungeonLevel)
    {
        var baseGold = Math.Max(
            _settings.LootBaseGold + dungeonLevel,
            mobAttackPower / _settings.LootPowerDivisor + dungeonLevel * _settings.LootLevelMultiplier
        );
        var variance = baseGold / _settings.LootVarianceDivisor;
        return _rng.Next(baseGold, baseGold + variance + 1);
    }

    /// <inheritdoc />
    public bool RollChance(int successChance)
    {
        return _rng.Next(0, 100) < successChance;
    }

    /// <inheritdoc />
    public T? SelectRandomTarget<T>(IEnumerable<T> targets) where T : class, IAlive
    {
        return _rng.GetRandomAlive(targets.ToList());
    }

    /// <inheritdoc />
    public PlayerState? SelectMobTarget(IReadOnlyList<PlayerState> players)
    {
        var alivePlayers = players.Where(p => p.IsAlive).ToList();
        if (alivePlayers.Count == 0)
        {
            return null;
        }

        // Chance to target lowest HP player
        if (RollChance(_settings.MobTargetLowestHpChance))
        {
            var minHp = alivePlayers.Min(p => p.CurrentHp);
            return alivePlayers.FirstOrDefault(p => p.CurrentHp == minHp);
        }

        return _rng.GetRandomAlive(alivePlayers);
    }
}