using Libertas.Discord.Adventure.Core.GameModels;
using Microsoft.Extensions.Logging;

namespace Libertas.Discord.Adventure.Core.Services.Phases;

/// <summary>
///     Handles the bot injection phase of the round, ensuring the party meets the minimum size.
/// </summary>
public class BotPhaseProcessor(IBotService botService, ILogger<BotPhaseProcessor> logger)
{
    private readonly IBotService _botService = botService;
    private readonly ILogger<BotPhaseProcessor> _logger = logger;

    /// <summary>
    ///     Injects AI companion bots if the party is below the minimum size.
    /// </summary>
    public List<PlayerState> InjectBotsIfNeeded(List<PlayerState> playerList, int level)
    {
        var generatedBots = _botService.GenerateBotsForParty(playerList, level);

        if (generatedBots.Count > 0)
        {
            playerList.AddRange(generatedBots);

            _logger.LogDebug(
                "Injected {BotCount} AI companions: {BotNames}",
                generatedBots.Count,
                string.Join(", ", generatedBots.Select(b => b.Name)));
        }

        return generatedBots;
    }
}