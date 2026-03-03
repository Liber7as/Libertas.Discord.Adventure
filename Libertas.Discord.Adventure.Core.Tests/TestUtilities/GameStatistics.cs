namespace Libertas.Discord.Adventure.Core.Tests.TestUtilities;

/// <summary>
///     Statistics collected during a game run.
/// </summary>
public class GameStatistics
{
    public int TotalRounds { get; set; }
    public int TotalAttacks { get; set; }
    public int CriticalHits { get; set; }
    public int MobsKilled { get; set; }
    public int PlayersKilled { get; set; }
    public int HealsPerformed { get; set; }
    public int DivineSmites { get; set; }
    public int SuccessfulEscapes { get; set; }
}