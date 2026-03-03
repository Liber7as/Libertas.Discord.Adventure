namespace Libertas.Discord.Adventure.Data.Entities;

/// <summary>
///     Persistent entity representing a mob preset used to spawn mobs in-game.
/// </summary>
public class MobPreset : IEntity
{
    /// <summary>
    ///     Display name of the mob preset.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Optional image URL representing the mob.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    ///     Minimum health for randomized mob instances from this preset.
    /// </summary>
    public int MinHealth { get; set; }

    /// <summary>
    ///     Maximum health for randomized mob instances from this preset.
    /// </summary>
    public int MaxHealth { get; set; }

    /// <summary>
    ///     Minimum attack power for randomized mob instances.
    /// </summary>
    public int MinAttackPower { get; set; }

    /// <summary>
    ///     Maximum attack power for randomized mob instances.
    /// </summary>
    public int MaxAttackPower { get; set; }

    /// <summary>
    ///     Primary key identifier.
    /// </summary>
    public int Id { get; set; }
}