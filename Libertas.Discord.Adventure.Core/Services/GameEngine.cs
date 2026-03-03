using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Phases;
using Microsoft.Extensions.Logging;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
///     Core game engine that orchestrates combat rounds.
///     Coordinates player actions, bot AI, mob attacks, and round summaries.
/// </summary>
/// <remarks>
///     Creates a new GameEngine instance.
/// </remarks>
/// <param name="playerPhaseProcessor">Processor for handling player actions.</param>
/// <param name="botPhaseProcessor">Processor for handling bot-related logic.</param>
/// <param name="mobPhaseProcessor">Processor for handling mob actions.</param>
/// <param name="roundSummaryGenerator">Service for generating round summary messages.</param>
/// <param name="logger">Logger for combat events and diagnostics.</param>
public class GameEngine(
    PlayerPhaseProcessor playerPhaseProcessor,
    BotPhaseProcessor botPhaseProcessor,
    MobPhaseProcessor mobPhaseProcessor,
    RoundSummaryGenerator roundSummaryGenerator,
    ILogger<GameEngine> logger) : IGameEngine
{
    private readonly BotPhaseProcessor _botPhaseProcessor = botPhaseProcessor;
    private readonly ILogger<GameEngine> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly MobPhaseProcessor _mobPhaseProcessor = mobPhaseProcessor;
    private readonly PlayerPhaseProcessor _playerPhaseProcessor = playerPhaseProcessor;
    private readonly RoundSummaryGenerator _roundSummaryGenerator = roundSummaryGenerator;

    /// <inheritdoc />
    public Task<RoundResult> ExecuteRoundAsync(
        int level,
        IEnumerable<PlayerState> players,
        IDictionary<PlayerId, PlayerAction> playerActions,
        IEnumerable<MobState> mobs,
        CancellationToken cancellationToken = default)
    {
        // Create defensive copies to avoid mutating caller's collections
        var playerList = players.ToList();
        var mobList = mobs.ToList();

        _logger.LogDebug(
            "Starting round at level {Level} with {PlayerCount} players and {MobCount} mobs",
            level, playerList.Count, mobList.Count);

        // Snapshot earnings before this round for delta calculation in round summary
        var earningsBefore = CaptureEarningsSnapshot(playerList);

        // --- Bot Injection Phase ---
        _botPhaseProcessor.InjectBotsIfNeeded(playerList, level);

        // Calculate party composition for status message
        var humanCount = playerList.Count(p => !p.IsBot && p.IsAlive);
        var botCount = playerList.Count(p => p.IsBot && p.IsAlive);
        var aliveMobCount = mobList.Count(m => m.IsAlive);

        var messages = new List<string>
        {
            FormatPartyStatusMessage(humanCount, botCount, aliveMobCount)
        };

        // --- Player Phase ---
        _playerPhaseProcessor.Execute(playerList, mobList, playerActions, level, messages);

        // --- Mob Phase ---
        _mobPhaseProcessor.Execute(playerList, mobList, level, messages);

        // --- Round Summary Phase ---
        RoundSummaryGenerator.AddSummary(playerList, earningsBefore, messages);

        // Log round completion statistics
        var alivePlayersAfter = playerList.Count(p => p.IsAlive);
        var aliveMobsAfter = mobList.Count(m => m.IsAlive);

        _logger.LogInformation(
            "Completed round at level {Level}: {AlivePlayers}/{TotalPlayers} players alive, {AliveMobs}/{TotalMobs} mobs alive",
            level, alivePlayersAfter, playerList.Count, aliveMobsAfter, mobList.Count);

        var result = new RoundResult
        {
            Level = level,
            Players = playerList,
            Mobs = mobList,
            Messages = messages
        };

        return Task.FromResult(result);
    }

    /// <summary>
    ///     Captures a snapshot of player earnings before the round for delta calculation.
    ///     Only tracks human players (bots don't earn rewards).
    /// </summary>
    private static Dictionary<PlayerId, RoundSummaryGenerator.EarningsSnapshot> CaptureEarningsSnapshot(List<PlayerState> players)
    {
        return players
            .Where(p => !p.IsBot)
            .ToDictionary(
                p => p.Id,
                p => new RoundSummaryGenerator.EarningsSnapshot(
                    p.GoldEarned,
                    p.MobsKilled,
                    p.AttackXpEarned,
                    p.MagicXpEarned,
                    p.SpeechXpEarned,
                    p.DefenseXpEarned));
    }

    /// <summary>
    ///     Formats the party status message shown at the start of each round.
    /// </summary>
    private static string FormatPartyStatusMessage(int humanCount, int botCount, int mobCount)
    {
        return botCount > 0
            ? $"{humanCount} adventurer(s) and {botCount} companion(s) face {mobCount} mob(s)."
            : $"{humanCount + botCount} player(s) and {mobCount} mob(s) present.";
    }
}