using Libertas.Discord.Adventure.Core.GameModels;
using Microsoft.Extensions.Logging;

namespace Libertas.Discord.Adventure.Core.Services.Phases;

/// <summary>
///     Handles the player phase of the round, where players and bots take actions.
/// </summary>
public class PlayerPhaseProcessor(
    IBotService botService,
    IActionResolutionService actionResolutionService,
    ILogger<PlayerPhaseProcessor> logger)
{
    private readonly IActionResolutionService _actionResolutionService = actionResolutionService;
    private readonly IBotService _botService = botService;
    private readonly ILogger<PlayerPhaseProcessor> _logger = logger;

    /// <summary>
    ///     Executes all player actions in order. Dead players are skipped.
    ///     Bots use AI to decide their actions; humans use provided actions or default to Run.
    /// </summary>
    public void Execute(
        List<PlayerState> playerList,
        List<MobState> mobList,
        IDictionary<PlayerId, PlayerAction> playerActions,
        int level,
        List<string> messages)
    {
        var alivePlayers = playerList.Where(p => p.IsAlive).ToList();

        foreach (var player in alivePlayers)
        {
            PlayerAction action;
            if (player.IsBot)
            {
                action = _botService.DecideBotAction(player, playerList, mobList);
                _logger.LogTrace("Bot {BotName} chose action {Action}", player.Name, action);
            }
            else
            {
                action = playerActions.TryGetValue(player.Id, out var providedAction)
                    ? providedAction
                    : PlayerAction.Run;

                if (!playerActions.ContainsKey(player.Id))
                {
                    _logger.LogDebug(
                        "Player {PlayerName} ({PlayerId}) did not submit an action; defaulting to Run",
                        player.Name, player.Id.Value);
                }
            }

            var context = new CombatContext<PlayerState>(level, player, playerList, mobList, messages);
            _actionResolutionService.HandlePlayerAction(context, action);
        }
    }
}