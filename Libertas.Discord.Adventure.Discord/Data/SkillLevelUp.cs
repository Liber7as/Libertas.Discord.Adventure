namespace Libertas.Discord.Adventure.Discord.Data;

/// <summary>
///     Represents a skill level-up event.
/// </summary>
public record SkillLevelUp(string SkillName, int OldLevel, int NewLevel);