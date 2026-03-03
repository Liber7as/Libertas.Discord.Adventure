using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests.TestUtilities;

/// <summary>
/// Enhanced test runner with detailed output formatting and statistics tracking.
/// Provides visual output for manual verification of game behavior.
/// </summary>
public class TestGameRunner(IGameEngine engine)
{
    private readonly IGameEngine _engine = engine ?? throw new ArgumentNullException(nameof(engine));

    /// <summary>
    /// Statistics collected during game execution.
    /// </summary>
    public GameStatistics Statistics { get; private set; } = new();

    /// <summary>
    /// Executes rounds until all players are dead or the max number of rounds is reached.
    /// Collects statistics and outputs detailed round information.
    /// </summary>
    public async Task<GameResult> ExecuteGameLoopAsync(
        int startLevel,
        List<PlayerState> players,
        List<MobState> mobs,
        IDictionary<PlayerId, PlayerAction> actions,
        int maxRounds = 100,
        Func<int, IEnumerable<PlayerState>, IEnumerable<MobState>, IDictionary<PlayerId, PlayerAction>?>? actionChooser = null,
        Func<IEnumerable<MobState>, int, List<MobState>?>? respawnFunc = null,
        bool verbose = true)
    {
        Statistics = new GameStatistics();
        var level = startLevel;
        var playerList = players.ToList();
        var mobList = mobs.ToList();

        if (verbose)
        {
            WriteHeader("GAME START");
            WritePlayers(playerList);
            WriteMobs(mobList);
        }

        while (playerList.Any(p => p.IsAlive) && level <= maxRounds)
        {
            var preRoundMobs = mobList.ToList();
            var preRoundPlayers = playerList.ToList();

            // Determine actions for this round
            var currentActions = actionChooser?.Invoke(level, playerList, mobList)
                                 ?? new Dictionary<PlayerId, PlayerAction>(actions);

            // Execute round
            var result = await _engine.ExecuteRoundAsync(level, playerList, currentActions, mobList);

            // Update state
            playerList = [.. result.Players];
            mobList = [.. result.Mobs];

            // Track statistics
            Statistics.TotalRounds++;
            TrackRoundStatistics(result, preRoundPlayers, preRoundMobs);

            if (verbose)
            {
                WriteRound(level, result, playerList, mobList);
            }

            // Respawn mobs if needed
            if (!mobList.Any(m => m.IsAlive) && respawnFunc != null)
            {
                var newMobs = respawnFunc(preRoundMobs, level);
                if (newMobs is { Count: > 0 })
                {
                    mobList = newMobs;
                    Statistics.MobsKilled++;

                    if (verbose)
                    {
                        TestContext.WriteLine($"  ? New mob spawns: {string.Join(", ", newMobs.Select(m => m.Name))}");
                    }
                }
            }

            level++;
        }

        var gameResult = new GameResult
        {
            FinalLevel = level - 1,
            SurvivingPlayers = [.. playerList.Where(p => p.IsAlive && !p.IsBot)],
            SurvivingBots = [.. playerList.Where(p => p.IsAlive && p.IsBot)],
            TotalGoldEarned = playerList.Where(p => !p.IsBot).Sum(p => p.GoldEarned),
            Statistics = Statistics
        };

        if (verbose)
        {
            WriteGameSummary(gameResult);
        }

        return gameResult;
    }

    /// <summary>
    /// Executes a single round and returns the result. Good for isolated action testing.
    /// </summary>
    public async Task<RoundResult> ExecuteSingleRoundAsync(
        int level,
        List<PlayerState> players,
        List<MobState> mobs,
        IDictionary<PlayerId, PlayerAction> actions,
        bool verbose = false)
    {
        var result = await _engine.ExecuteRoundAsync(level, players, actions, mobs);

        if (verbose)
        {
            WriteRound(level, result, [.. result.Players], [.. result.Mobs]);
        }

        return result;
    }

    #region Statistics Tracking

    private void TrackRoundStatistics(RoundResult result, List<PlayerState> prePlayers, List<MobState> preMobs)
    {
        foreach (var message in result.Messages)
        {
            // Track crits
            if (message.Contains("CRIT", StringComparison.OrdinalIgnoreCase))
            {
                Statistics.CriticalHits++;
            }

            // Track kills
            if (message.Contains("kills", StringComparison.OrdinalIgnoreCase))
            {
                if (preMobs.Any(m => message.Contains(m.Name)))
                {
                    Statistics.MobsKilled++;
                }
                else
                {
                    Statistics.PlayersKilled++;
                }
            }

            // Track escapes
            if (message.Contains("ESCAPE", StringComparison.OrdinalIgnoreCase))
            {
                Statistics.SuccessfulEscapes++;
            }

            // Track smites
            if (message.Contains("SMITE", StringComparison.OrdinalIgnoreCase))
            {
                Statistics.DivineSmites++;
            }

            // Track heals
            if (message.Contains("heals", StringComparison.OrdinalIgnoreCase))
            {
                Statistics.HealsPerformed++;
            }
        }

        // Track attacks (non-crit hits)
        Statistics.TotalAttacks += result.Messages.Count(m =>
            m.Contains("attacks", StringComparison.OrdinalIgnoreCase) ||
            m.Contains("casts magic", StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region Output Formatting

    private static void WriteHeader(string title)
    {
        TestContext.WriteLine();
        TestContext.WriteLine(new string('?', 60));
        TestContext.WriteLine($"  {title}");
        TestContext.WriteLine(new string('?', 60));
    }

    private static void WritePlayers(List<PlayerState> players)
    {
        TestContext.WriteLine("\n  PARTY:");
        foreach (var p in players.Where(p => !p.IsBot))
        {
            TestContext.WriteLine($"    • {p.Name,-15} HP: {p.CurrentHp,3}/{p.MaxHp,-3}  " +
                                  $"ATK:{p.AttackPower.Value,2} MAG:{p.MagicPower.Value,2} " +
                                  $"SPE:{p.SpeechPower.Value,2} DEF:{p.DefensePower.Value,2}");
        }

        var bots = players.Where(p => p.IsBot).ToList();
        if (bots.Count > 0)
        {
            TestContext.WriteLine("  COMPANIONS:");
            foreach (var b in bots)
            {
                TestContext.WriteLine($"    • {b.Name,-15} HP: {b.CurrentHp,3}/{b.MaxHp,-3}  [BOT]");
            }
        }
    }

    private static void WriteMobs(List<MobState> mobs)
    {
        TestContext.WriteLine("\n  ENEMIES:");
        foreach (var m in mobs)
        {
            TestContext.WriteLine($"    • {m.Name,-15} HP: {m.CurrentHp,3}/{m.MaxHp,-3}  ATK:{m.AttackPower.Value,2}");
        }
    }

    private static void WriteRound(int level, RoundResult result, List<PlayerState> players, List<MobState> mobs)
    {
        TestContext.WriteLine();
        TestContext.WriteLine($"?? ROUND {level} ??????????????????????????????????????????");

        foreach (var msg in result.Messages)
        {
            TestContext.WriteLine($"? {msg}");
        }

        // Status after round
        var alivePlayers = players.Where(p => p.IsAlive && !p.IsBot).ToList();
        var aliveBots = players.Where(p => p.IsAlive && p.IsBot).ToList();
        var aliveMobs = mobs.Where(m => m.IsAlive).ToList();

        TestContext.WriteLine("?");
        TestContext.WriteLine($"? Status: {alivePlayers.Count} players, {aliveBots.Count} bots, {aliveMobs.Count} mobs alive");

        if (alivePlayers.Count > 0)
        {
            var hpSummary = string.Join(", ", alivePlayers.Select(p => $"{p.Name}:{p.CurrentHp}hp"));
            TestContext.WriteLine($"? HP: {hpSummary}");
        }

        TestContext.WriteLine($"????????????????????????????????????????????????????????");
    }

    private static void WriteGameSummary(GameResult result)
    {
        WriteHeader("GAME OVER");

        TestContext.WriteLine($"\n  Final Level Reached: {result.FinalLevel}");
        TestContext.WriteLine($"  Surviving Players:   {result.SurvivingPlayers.Count}");
        TestContext.WriteLine($"  Total Gold Earned:   {result.TotalGoldEarned:F0}");

        TestContext.WriteLine("\n  STATISTICS:");
        TestContext.WriteLine($"    • Total Rounds:      {result.Statistics.TotalRounds}");
        TestContext.WriteLine($"    • Total Attacks:     {result.Statistics.TotalAttacks}");
        TestContext.WriteLine($"    • Critical Hits:     {result.Statistics.CriticalHits}");
        TestContext.WriteLine($"    • Mobs Killed:       {result.Statistics.MobsKilled}");
        TestContext.WriteLine($"    • Players Killed:    {result.Statistics.PlayersKilled}");
        TestContext.WriteLine($"    • Heals Performed:   {result.Statistics.HealsPerformed}");
        TestContext.WriteLine($"    • Divine Smites:     {result.Statistics.DivineSmites}");
        TestContext.WriteLine($"    • Successful Escapes:{result.Statistics.SuccessfulEscapes}");

        if (result.Statistics.TotalAttacks > 0)
        {
            var critRate = (double)result.Statistics.CriticalHits / result.Statistics.TotalAttacks * 100;
            TestContext.WriteLine($"    • Crit Rate:         {critRate:F1}%");
        }

        if (result.SurvivingPlayers.Count > 0)
        {
            TestContext.WriteLine("\n  SURVIVORS:");
            foreach (var p in result.SurvivingPlayers)
            {
                TestContext.WriteLine($"    • {p.Name,-15} HP: {p.CurrentHp}/{p.MaxHp}  Gold: {p.GoldEarned:F0}");
            }
        }

        TestContext.WriteLine(new string('?', 60));
    }

    #endregion

    #region Legacy Compatibility

    /// <summary>
    /// Legacy method for backward compatibility with existing tests.
    /// </summary>
    public async Task<int> ExecuteGameLoopUntilPlayersDeadAsync(
        int startLevel,
        List<PlayerState> playersList,
        List<MobState> mobsList,
        IDictionary<PlayerId, PlayerAction> actions,
        int maxRounds = 100,
        Func<int, IEnumerable<PlayerState>, IEnumerable<MobState>, IDictionary<PlayerId, PlayerAction>?>? actionChooser = null,
        Func<IEnumerable<MobState>, int, List<MobState>?>? respawnFunc = null)
    {
        var result = await ExecuteGameLoopAsync(
            startLevel, playersList, mobsList, actions, maxRounds, actionChooser, respawnFunc, verbose: true);

        // Update the mutable lists for backward compatibility
        playersList.Clear();
        playersList.AddRange(result.SurvivingPlayers);
        playersList.AddRange(result.SurvivingBots);

        return result.FinalLevel + 1;
    }

    #endregion
}