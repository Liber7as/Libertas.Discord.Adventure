using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Tests.TestUtilities;

/// <summary>
/// Result of a complete game run.
/// </summary>
public class GameResult
{
    public required int FinalLevel { get; init; }
    public required List<PlayerState> SurvivingPlayers { get; init; }
    public required List<PlayerState> SurvivingBots { get; init; }
    public required double TotalGoldEarned { get; init; }
    public required GameStatistics Statistics { get; init; }
}