using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
///     Service for generating and managing AI companion bots.
///     Bots fill party slots when there aren't enough human players.
/// </summary>
public interface IBotService
{
    /// <summary>
    ///     Generates bot players to fill the party to the minimum size.
    ///     Bot stats are scaled based on the dungeon level with configurable variance.
    /// </summary>
    /// <param name="allPlayers">Current list of all players (humans and existing bots).</param>
    /// <param name="dungeonLevel">Current dungeon level, used to scale bot stats.</param>
    /// <returns>List of new bot players to add to the party. Empty if party is already full.</returns>
    List<PlayerState> GenerateBotsForParty(IReadOnlyList<PlayerState> allPlayers, int dungeonLevel);

    /// <summary>
    ///     Determines the action a bot should take based on the current combat state.
    ///     Uses a simple AI decision tree prioritizing support actions when needed.
    /// </summary>
    /// <param name="bot">The bot player making the decision.</param>
    /// <param name="allPlayers">All players in the combat (including bots).</param>
    /// <param name="mobs">All mobs in the combat.</param>
    /// <returns>The action the bot should take this turn.</returns>
    PlayerAction DecideBotAction(PlayerState bot, IReadOnlyList<PlayerState> allPlayers, IReadOnlyList<MobState> mobs);
}