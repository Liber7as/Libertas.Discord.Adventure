namespace Libertas.Discord.Adventure.Core.GameModels;

/// <summary>
/// Lightweight DTO representing a mob preset for use by core logic.
/// Keeps core independent from EF entity types.
/// </summary>
public record MobPresetDto(
    int Id,
    string Name,
    int MinHealth,
    int MaxHealth,
    int MinAttackPower,
    int MaxAttackPower,
    string? ImageUrl
);
