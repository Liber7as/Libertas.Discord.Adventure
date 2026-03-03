namespace Libertas.Discord.Adventure.Core.GameModels;

/// <summary>
///     Represents a mob's state during combat.
///     This is a mutable class that changes during combat rounds.
/// </summary>
public class MobState : IAlive
{
    /// <summary>
    ///     Unique identifier for this mob instance or preset mapping.
    /// </summary>
    public MobId Id { get; init; } = new(0);

    /// <summary>
    ///     Display name of the mob.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    ///     Maximum hit points for the mob.
    /// </summary>
    public int MaxHp { get; set; }

    /// <summary>
    ///     Current hit points during combat.
    /// </summary>
    public int CurrentHp { get; set; }

    /// <summary>
    ///     Offensive power level used when calculating damage.
    /// </summary>
    public PowerLevel AttackPower { get; set; } = new(1);

    /// <summary>
    ///     Optional URL for an image representing this mob.
    /// </summary>
    public string? ImageUrl { get; init; }

    /// <summary>
    ///     Whether the mob is alive (CurrentHp &gt; 0).
    /// </summary>
    public bool IsAlive => CurrentHp > 0;
}