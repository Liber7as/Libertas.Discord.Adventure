namespace Libertas.Discord.Adventure.Discord.Data;

/// <summary>
/// Result of saving player progress, including any level-ups.
/// </summary>
public record SaveProgressResult(List<SkillLevelUp> LevelUps);