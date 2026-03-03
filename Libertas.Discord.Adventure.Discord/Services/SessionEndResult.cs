using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Discord.Data;

namespace Libertas.Discord.Adventure.Discord.Services;

/// <summary>
///     Result of ending a session, including level-up information for all players.
/// </summary>
/// <param name="Session">The ended session with final state.</param>
/// <param name="LevelUps">Dictionary mapping player IDs to their skill level-ups during the session.</param>
public record SessionEndResult(AdventureSession Session, Dictionary<PlayerId, List<SkillLevelUp>> LevelUps);