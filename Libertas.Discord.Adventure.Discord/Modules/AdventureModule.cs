using Discord.Commands;
using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Discord.Data;
using Libertas.Discord.Adventure.Discord.Services;

namespace Libertas.Discord.Adventure.Discord.Modules;

/// <summary>
/// Discord command module for starting adventures, running simulations, and viewing player stats.
/// Handles the main adventure loop and player interaction commands.
/// </summary>
[Group("a")]
public class AdventureModule(
    AdventureSessionManager sessionManager,
    MessageService messageService,
    IRandomNumberGenerator randomNumberGenerator,
    IMobFactoryService mobFactoryService,
    IPlayerService playerService,
    IPlayerProgressionService progressionService) : ModuleBase<SocketCommandContext>
{
    private readonly MessageService _messageService = messageService;
    private readonly IMobFactoryService _mobFactoryService = mobFactoryService;
    private readonly IRandomNumberGenerator _randomNumberGenerator = randomNumberGenerator;
    private readonly AdventureSessionManager _sessionManager = sessionManager;
    private readonly IPlayerService _playerService = playerService;
    private readonly IPlayerProgressionService _progressionService = progressionService;

    /// <summary>
    /// Starts a new adventure session in the current channel.
    /// </summary>
    [Command("start")]
    [Summary("Start a button-driven adventure in the channel.")]
    public async Task StartAsync()
    {
        var channelId = Context.Channel.Id;

        // If a session already exists for this channel, inform the user and do not start a new one.
        if (_sessionManager.Sessions.TryGetValue(channelId, out var existingSession))
        {
            await ReplyAsync($"Cannot start an adventure: a session is already running in this channel (level {existingSession.Level}).");
            return;
        }

        // Start a new session at level 1
        const int startingLevel = 1;
        var session = await _sessionManager.StartSessionAsync(channelId, startingLevel);

        try
        {
            // Track used preset ids to prefer different presets each round
            var usedPresetIds = new HashSet<int>();

            // Use factory to create initial mob for the session
            var mob = await _mobFactoryService.CreateRandomMobForLevelAsync(session.Level, null, CancellationToken.None);

            if (mob == null)
            {
                await ReplyAsync("Failed to start adventure: no mob presets are available.");

                await _sessionManager.EndSessionAsync(session.ChannelId);

                return;
            }

            usedPresetIds.Add(mob.Id.Value);

            // Add the created mob to the session's mob list
            session.Mobs = [mob];

            // Send the initial round information (use a short timer for the first round)
            const int roundTimer = 60;

            var round = 0;
            RoundResult? result = null;

            // Main round loop: continue while any human player is alive
            do
            {
                await _messageService.DeleteCombatMessageAsync(session);

                round++;

                await _messageService.SendRoundInformationAsync(session, round, DateTimeOffset.UtcNow.AddSeconds(roundTimer));
                await _messageService.SendCombatMessageAsync(session);

                await Task.Delay(TimeSpan.FromSeconds(roundTimer));

                await _messageService.DeleteCombatMessageAsync(session);

                if (session.Players.Any(p => p.IsAlive))
                {
                    result = await _sessionManager.ExecuteRoundAsync(session);
                }

                if (result == null)
                {
                    throw new Exception("Round execution failed.");
                }

                // Check if any humans are still in the game (alive or dead — dead still count)
                var humansRemaining = result.Players.Any(p => !p.IsBot);

                await _messageService.PostRoundResultsAsync(session, result, DateTimeOffset.UtcNow.AddSeconds(roundTimer));

                // If no humans remain at all (all ran away), end immediately
                if (!humansRemaining)
                {
                    await Context.Channel.SendMessageAsync("All adventurers have fled! The dungeon awaits its next challengers...");
                    return;
                }

                // If all mobs are dead, advance the level and spawn a new mob
                if (!result.Mobs.Any(m => m.IsAlive))
                {
                    round = 0;
                    session.Level++;

                    var nextMob = await _mobFactoryService.CreateRandomMobForLevelAsync(session.Level, usedPresetIds, CancellationToken.None);

                    if (nextMob == null)
                    {
                        await Context.Channel.SendMessageAsync("The dungeon has run out of challenges. You win... for now.");
                        return;
                    }

                    usedPresetIds.Add(nextMob.Id.Value);

                    session.Mobs.Clear();
                    session.Mobs.Add(nextMob);
                }

                await Task.Delay(TimeSpan.FromSeconds(roundTimer));

                // Continue while any human player is alive
            } while (session.Players.Any(p => !p.IsBot && p.IsAlive));

            await Context.Channel.SendMessageAsync("💀 All adventurers have fallen. The dungeon claims another party...");
        }
        finally
        {
            await _messageService.DeleteCombatMessageAsync(session);
            await _sessionManager.EndSessionAsync(channelId);
        }
    }

    /// <summary>
    /// Runs a headless combat simulation of 4 players vs respawning mobs.
    /// </summary>
    [Command("simulate")]
    [Summary("Run a headless combat simulation of 4 players vs respawning mobs.")]
    [RequireOwner]
    public async Task SimulateAsync()
    {
        // Create four players for simulation
        var initialPlayers = new List<PlayerState>
        {
            CreatePlayer(1, "Alice"),
            CreatePlayer(2, "Bob"),
            CreatePlayer(3, "Carol"),
            CreatePlayer(4, "Dave")
        };

        // Start session via manager
        var session = await _sessionManager.StartSessionAsync(Context.Channel.Id, 1);

        // Track used preset ids to prefer different presets each round
        var usedPresetIds = new HashSet<int>();

        // Create initial mob using factory
        var initialMob = await _mobFactoryService.CreateRandomMobForLevelAsync(session.Level, null, CancellationToken.None);

        if (initialMob == null)
        {
            await ReplyAsync("Failed to simulate: no mob presets are configured.");
            await _sessionManager.EndSessionAsync(session.ChannelId);
            return;
        }

        usedPresetIds.Add(initialMob.Id.Value);

        session.Players = [.. initialPlayers];
        session.Mobs = [initialMob];

        // Possible actions for simulation (excluding Run)
        var possibleChoices = Enum.GetValues<PlayerAction>().Where(action => action != PlayerAction.Run).ToArray();

        const int roundTimer = 30;
        var round = 0;
        RoundResult result;
        // Main simulation loop: continue while any player is alive
        do
        {
            round++;

            // Randomly select actions for each player
            foreach (var player in session.Players)
            {
                await _sessionManager.RecordActionAsync(session.ChannelId, player.Id, possibleChoices.OrderBy(_ => _randomNumberGenerator.Next(int.MinValue, int.MaxValue)).First());
            }

            await _messageService.SendRoundInformationAsync(session, round, DateTimeOffset.UtcNow.AddSeconds(roundTimer));

            result = await _sessionManager.ExecuteRoundAsync(session);

            await _messageService.PostRoundResultsAsync(session, result, DateTimeOffset.UtcNow.AddSeconds(roundTimer));

            // If all mobs are dead, advance the level and spawn a new mob
            if (!result.Mobs.Any(m => m.IsAlive))
            {
                round = 0;
                session.Level++;

                // Use factory to create next mob excluding used presets
                var nextMob = await _mobFactoryService.CreateRandomMobForLevelAsync(session.Level, usedPresetIds, CancellationToken.None);

                if (nextMob == null)
                {
                    await ReplyAsync("Simulation aborted: no unused mob presets available.");
                    await _sessionManager.EndSessionAsync(session.ChannelId);
                    return;
                }

                usedPresetIds.Add(nextMob.Id.Value);

                session.Mobs.Clear();
                session.Mobs.Add(nextMob);
            }

            await Task.Delay(TimeSpan.FromSeconds(roundTimer));
        } while (result.Players.Any(p => p.IsAlive));

        await _sessionManager.EndSessionAsync(session.ChannelId);

        await Context.Channel.SendMessageAsync("All players are dead. Simulation ended.");

        // Helper to create a random player with plausible stats
        PlayerState CreatePlayer(ulong id, string name)
        {
            var maxHp = _randomNumberGenerator.Next(20, 41); // 20-40 HP
            return new PlayerState
            {
                Id = new PlayerId(id),
                Name = name,
                MaxHp = maxHp,
                CurrentHp = maxHp,
                AttackPower = new PowerLevel(_randomNumberGenerator.Next(3, 11)), // 3-10
                MagicPower = new PowerLevel(_randomNumberGenerator.Next(1, 9)), // 1-8
                SpeechPower = new PowerLevel(_randomNumberGenerator.Next(1, 7)), // 1-6
                DefensePower = new PowerLevel(_randomNumberGenerator.Next(1, 6)) // 1-5
            };
        }
    }

    /// <summary>
    /// Displays the current player's skills, XP, and lifetime statistics.
    /// </summary>
    [Command("stats")]
    [Summary("View your character's skills, XP, and lifetime statistics.")]
    public async Task StatsAsync()
    {
        var player = await _playerService.GetPlayerDataAsync(Context.User.Id);

        if (player == null)
        {
            await ReplyAsync("You haven't adventured yet! Use `a start` to begin your journey.");
            return;
        }

        // Calculate XP progress for each skill using persistent totals and skill levels
        var attackXpNext = _progressionService.GetXpToNextSkillLevel(player.Skills.AttackLevel);
        var magicXpNext = _progressionService.GetXpToNextSkillLevel(player.Skills.MagicLevel);
        var speechXpNext = _progressionService.GetXpToNextSkillLevel(player.Skills.SpeechLevel);
        var defenseXpNext = _progressionService.GetXpToNextSkillLevel(player.Skills.DefenseLevel);

        var attackXpCurrent = player.AttackXpTotal - _progressionService.GetXpRequiredForSkillLevel(player.Skills.AttackLevel);
        var magicXpCurrent = player.MagicXpTotal - _progressionService.GetXpRequiredForSkillLevel(player.Skills.MagicLevel);
        var speechXpCurrent = player.SpeechXpTotal - _progressionService.GetXpRequiredForSkillLevel(player.Skills.SpeechLevel);
        var defenseXpCurrent = player.DefenseXpTotal - _progressionService.GetXpRequiredForSkillLevel(player.Skills.DefenseLevel);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"## {player.Name}'s Stats");
        sb.AppendLine("```");
        sb.AppendLine($"  Combat Level: {player.CombatLevel}    Total Level: {player.TotalLevel}");
        sb.AppendLine();
        sb.AppendLine("  Skills:");
        sb.AppendLine($"    Attack  Lv.{player.Skills.AttackLevel,-3}  ({attackXpCurrent}/{attackXpNext} XP)");
        sb.AppendLine($"    Magic   Lv.{player.Skills.MagicLevel,-3}  ({magicXpCurrent}/{magicXpNext} XP)");
        sb.AppendLine($"    Speech  Lv.{player.Skills.SpeechLevel,-3}  ({speechXpCurrent}/{speechXpNext} XP)");
        sb.AppendLine($"    Defense Lv.{player.Skills.DefenseLevel,-3}  ({defenseXpCurrent}/{defenseXpNext} XP)");
        sb.AppendLine();
        sb.AppendLine($"  Gold: {player.TotalGold:F0}");
        sb.AppendLine($"  Kills: {player.TotalKills}   Deaths: {player.TotalDeaths}");
        sb.AppendLine($"  Highest Dungeon: Level {player.HighestDungeonLevel}");
        sb.AppendLine("```");

        await ReplyAsync(sb.ToString());
    }
}