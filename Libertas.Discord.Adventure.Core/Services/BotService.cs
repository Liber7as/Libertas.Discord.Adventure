using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
///     Default implementation of <see cref="IBotService" />.
///     Creates AI companions with level-scaled stats and simple combat AI.
/// </summary>
/// <remarks>
///     <para>
///         <b>Bot ID Strategy:</b> Bot IDs count down from <c>ulong.MaxValue</c> to avoid
///         collision with Discord user IDs (which are smaller snowflake values).
///     </para>
///     <para>
///         <b>AI Decision Tree:</b> Bots prioritize healing injured allies, then self-preservation,
///         then offensive actions weighted toward physical attacks.
///     </para>
/// </remarks>
/// <remarks>
///     Creates a new BotService instance.
/// </remarks>
/// <param name="rng">Random number generator for stat variance and AI decisions.</param>
/// <param name="options">Bot configuration settings.</param>
/// <param name="logger">Logger for bot generation and AI decision events.</param>
public class BotService(
    IRandomNumberGenerator rng,
    IOptions<BotSettings> options,
    ILogger<BotService> logger) : IBotService
{
    private readonly ILogger<BotService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IRandomNumberGenerator _rng = rng ?? throw new ArgumentNullException(nameof(rng));
    private readonly BotSettings _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    ///     Next bot ID to assign. Counts down from ulong.MaxValue to avoid Discord ID collisions.
    /// </summary>
    private ulong _nextBotId = ulong.MaxValue;

    /// <inheritdoc />
    public List<PlayerState> GenerateBotsForParty(IReadOnlyList<PlayerState> allPlayers, int dungeonLevel)
    {
        var bots = new List<PlayerState>();

        // Count alive humans and total bots (alive + dead) to determine if more bots are needed.
        // Dead bots should NOT be replaced � the party started with enough help.
        var aliveHumans = allPlayers.Count(p => !p.IsBot && p.IsAlive);
        var totalBots = allPlayers.Count(p => p.IsBot);
        var currentSize = aliveHumans + totalBots;
        var botsNeeded = _settings.MinimumPartySize - currentSize;

        if (botsNeeded <= 0)
        {
            _logger.LogTrace(
                "Party has {AliveHumans} alive humans and {TotalBots} bots (alive+dead), no new bots needed (minimum: {MinSize})",
                aliveHumans, totalBots, _settings.MinimumPartySize);
            return bots;
        }

        _logger.LogDebug(
            "Party has {AliveHumans} alive humans and {TotalBots} bots, generating {BotsNeeded} bot(s) to reach minimum of {MinSize}",
            aliveHumans, totalBots, botsNeeded, _settings.MinimumPartySize);

        // Build list of available names (exclude names already in use)
        var usedNames = allPlayers
            .Where(p => p.IsBot)
            .Select(p => p.Name)
            .ToHashSet();
        var availableNames = _settings.BotNames
            .Where(n => !usedNames.Contains(n))
            .ToList();

        for (var i = 0; i < botsNeeded; i++)
        {
            var bot = CreateBot(dungeonLevel, availableNames);
            bots.Add(bot);

            _logger.LogInformation(
                "Generated bot '{BotName}' (ID: {BotId}) at effective level {Level} with {Hp} HP and {Power} power",
                bot.Name, bot.Id.Value, dungeonLevel, bot.MaxHp, bot.AttackPower.Value);
        }

        return bots;
    }

    /// <inheritdoc />
    public PlayerAction DecideBotAction(PlayerState bot, IReadOnlyList<PlayerState> allPlayers, IReadOnlyList<MobState> mobs)
    {
        // AI Decision Tree:
        // Priority 1: If injured allies exist at <50% HP, 40% chance to heal them
        // Priority 2: If self is at <30% HP, 30% chance to pray (self-heal on failure)
        // Priority 3: Offensive actions weighted 70% attack, 20% magic, 10% talk

        // Check for injured allies (not self) below 50% HP
        var injuredAllies = allPlayers
            .Where(p => p.IsAlive && p.Id != bot.Id && p.CurrentHp < p.MaxHp * 0.5)
            .ToList();

        if (injuredAllies.Count > 0 && _rng.Next(0, 100) < 40)
        {
            _logger.LogTrace(
                "Bot {BotName} choosing Heal ({InjuredCount} injured allies)",
                bot.Name, injuredAllies.Count);
            return PlayerAction.Heal;
        }

        // Check if bot itself needs healing (below 30% HP)
        if (bot.CurrentHp < bot.MaxHp * 0.3 && _rng.Next(0, 100) < 30)
        {
            _logger.LogTrace(
                "Bot {BotName} choosing Pray (self HP: {CurrentHp}/{MaxHp}, {Percent}%)",
                bot.Name, bot.CurrentHp, bot.MaxHp, bot.CurrentHp * 100 / bot.MaxHp);
            return PlayerAction.Pray;
        }

        // Default to offensive actions with weighted distribution
        var roll = _rng.Next(0, 100);
        var action = roll switch
        {
            < 70 => PlayerAction.Attack, // 70% chance
            < 90 => PlayerAction.Magic, // 20% chance
            _ => PlayerAction.Talk // 10% chance
        };

        _logger.LogTrace("Bot {BotName} choosing {Action} (roll: {Roll})", bot.Name, action, roll);
        return action;
    }

    /// <summary>
    ///     Creates a single bot with stats scaled to the dungeon level.
    ///     Applies random variance to make bots feel more unique.
    /// </summary>
    /// <param name="dungeonLevel">Base level for stat scaling.</param>
    /// <param name="availableNames">Pool of unused bot names. Modified to remove selected name.</param>
    /// <returns>A new bot PlayerState ready for combat.</returns>
    private PlayerState CreateBot(int dungeonLevel, List<string> availableNames)
    {
        // Apply variance to the level: �StatLevelVariance
        var minLevel = Math.Max(_settings.MinimumBotLevel, dungeonLevel - _settings.StatLevelVariance);
        var maxLevel = dungeonLevel + _settings.StatLevelVariance;
        var effectiveLevel = _rng.Next(minLevel, maxLevel + 1);

        // Calculate stats: base + (level � per-level bonus)
        var hp = _settings.BaseHp + effectiveLevel * _settings.HpPerLevel;
        var power = _settings.BasePower + effectiveLevel * _settings.PowerPerLevel;

        var name = SelectBotName(availableNames);

        // Bot IDs count down from ulong.MaxValue to avoid collision with Discord user IDs
        var botId = new PlayerId(_nextBotId--);

        return new PlayerState
        {
            Id = botId,
            Name = name,
            MaxHp = hp,
            CurrentHp = hp,
            AttackPower = new PowerLevel(power),
            MagicPower = new PowerLevel(power),
            SpeechPower = new PowerLevel(power),
            DefensePower = new PowerLevel(power),
            IsBot = true
        };
    }

    /// <summary>
    ///     Selects a name for a new bot from the available pool.
    ///     Removes the selected name from the pool to prevent duplicates.
    ///     Falls back to a generic numbered name if all names are exhausted.
    /// </summary>
    /// <param name="availableNames">Pool of unused names. Modified to remove selected name.</param>
    /// <returns>The selected bot name.</returns>
    private string SelectBotName(List<string> availableNames)
    {
        if (availableNames.Count == 0)
        {
            // Fallback when all configured names are in use
            var fallbackName = $"Adventurer #{_rng.Next(1, 1000)}";
            _logger.LogDebug("All bot names in use, using fallback name: {Name}", fallbackName);
            return fallbackName;
        }

        var name = _rng.GetRandom(availableNames);

        if (name != null)
        {
            availableNames.Remove(name);
            return name;
        }

        // Shouldn't happen, but handle gracefully
        return $"Adventurer #{_rng.Next(1, 1000)}";
    }
}