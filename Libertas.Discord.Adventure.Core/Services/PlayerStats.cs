namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
/// Calculated combat stats for a player based on their skill levels.
/// </summary>
public record PlayerStats(
    int MaxHp,
    int AttackPower,
    int MagicPower,
    int SpeechPower,
    int DefensePower
);