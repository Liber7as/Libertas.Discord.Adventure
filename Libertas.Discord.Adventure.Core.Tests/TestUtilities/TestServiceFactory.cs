using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Core.Services.Actions;
using Libertas.Discord.Adventure.Core.Services.Phases;
using Libertas.Discord.Adventure.Core.Settings;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Libertas.Discord.Adventure.Core.Tests.TestUtilities;

/// <summary>
/// Factory for creating test instances of game services with consistent configuration.
/// Centralizes test setup to ensure all tests use the same baseline configuration.
/// Uses <see cref="NullLogger{T}"/> for all services to suppress log output during tests.
/// </summary>
public static class TestServiceFactory
{
    /// <summary>
    /// Creates a seeded random number generator for deterministic tests.
    /// </summary>
    /// <param name="seed">Optional seed for reproducible results. If null, uses a random seed.</param>
    public static IRandomNumberGenerator CreateRng(int? seed = null)
    {
        return seed.HasValue
            ? new SeededRandomNumberGenerator(seed.Value)
            : new RandomNumberGenerator();
    }

    /// <summary>
    /// Creates an action localization service with minimal test messages.
    /// </summary>
    public static IActionLocalizationService CreateLocalizationService(IRandomNumberGenerator? rng = null)
    {
        rng ??= new RandomNumberGenerator();
        return new ActionLocalizationService(rng, Options.Create(CreateTestLocalizationSettings()));
    }

    /// <summary>
    /// Creates a combat calculator with default test settings.
    /// </summary>
    public static ICombatCalculator CreateCombatCalculator(IRandomNumberGenerator? rng = null, CombatSettings? settings = null)
    {
        rng ??= new RandomNumberGenerator();
        settings ??= CreateTestCombatSettings();
        return new CombatCalculator(rng, Options.Create(settings));
    }

    /// <summary>
    /// Creates an action resolution service with test configuration using the strategy pattern.
    /// </summary>
    public static IActionResolutionService CreateActionResolutionService(
        IRandomNumberGenerator? rng = null,
        IPlayerProgressionService? progression = null,
        CombatSettings? combatSettings = null)
    {
        rng ??= new RandomNumberGenerator();
        progression ??= CreateProgressionService();
        var loc = CreateLocalizationService(rng);
        var calc = CreateCombatCalculator(rng, combatSettings);

        // Create shared combat helpers
        var xpDistributor = new Libertas.Discord.Adventure.Core.Services.Combat.XpDistributor(progression);
        var lootDistributor = new Libertas.Discord.Adventure.Core.Services.Combat.LootDistributor(calc);
        var damageApplier = new Libertas.Discord.Adventure.Core.Services.Combat.DamageApplier(progression);

        // Create all action handlers
        var handlers = new IPlayerActionHandler[]
        {
            new AttackActionHandler(calc, loc, progression, xpDistributor, lootDistributor, damageApplier),
            new MagicActionHandler(calc, loc, progression, xpDistributor, lootDistributor, damageApplier),
            new TalkActionHandler(calc, loc, progression, xpDistributor, lootDistributor, damageApplier),
            new PrayActionHandler(calc, loc, progression, xpDistributor, lootDistributor, damageApplier),
            new HealActionHandler(calc, loc, progression, xpDistributor),
            new RunActionHandler(calc, loc, progression, lootDistributor)
        };

        var mobHandler = new MobActionHandler(calc, loc, progression);

        return new ActionResolutionService(
            handlers,
            mobHandler,
            NullLogger<ActionResolutionService>.Instance);
    }

    /// <summary>
    /// Creates a bot service with test configuration.
    /// </summary>
    public static IBotService CreateBotService(IRandomNumberGenerator? rng = null, BotSettings? settings = null)
    {
        rng ??= new RandomNumberGenerator();
        settings ??= CreateTestBotSettings();
        return new BotService(
            rng,
            Options.Create(settings),
            NullLogger<BotService>.Instance);
    }

    /// <summary>
    /// Creates a fully configured game engine for testing.
    /// </summary>
    public static IGameEngine CreateGameEngine(IRandomNumberGenerator? rng = null, BotSettings? botSettings = null)
    {
        rng ??= new RandomNumberGenerator();
        var actionResolution = CreateActionResolutionService(rng);
        var botService = CreateBotService(rng, botSettings);

        // Create the new processors
        var playerPhaseProcessor = new PlayerPhaseProcessor(botService, actionResolution, NullLogger<PlayerPhaseProcessor>.Instance);
        var botPhaseProcessor = new BotPhaseProcessor(botService, NullLogger<BotPhaseProcessor>.Instance);
        var mobPhaseProcessor = new MobPhaseProcessor(actionResolution);
        var roundSummaryGenerator = new RoundSummaryGenerator();

        return new GameEngine(
            playerPhaseProcessor,
            botPhaseProcessor,
            mobPhaseProcessor,
            roundSummaryGenerator,
            NullLogger<GameEngine>.Instance);
    }

    /// <summary>
    /// Creates a game engine with no bot injection for isolated action testing.
    /// </summary>
    public static IGameEngine CreateGameEngineWithoutBots(IRandomNumberGenerator? rng = null)
    {
        var noBotSettings = new BotSettings { MinimumPartySize = 0 };
        return CreateGameEngine(rng, noBotSettings);
    }

    /// <summary>
    /// Default test combat settings matching the original magic numbers.
    /// </summary>
    public static CombatSettings CreateTestCombatSettings() => new()
    {
        CriticalHitChance = 15,
        CriticalHitMultiplier = 2.0,
        MobTargetLowestHpChance = 70,
        MobDamageScalingMultiplier = 1.05,
        DamageVarianceDivisor = 4,
        TalkPowerMultiplier = 1.0,
        TalkLevelDivisor = 2,
        PraySmiteBaseChance = 10,
        PraySmiteLevelThreshold = 20,
        PraySmiteMinChance = 5,
        PraySmiteMaxChance = 30,
        PrayHealMaxPercent = 25,
        HealPowerDivisor = 2,
        HealRandomBonusMin = 5,
        HealRandomBonusMax = 11,
        RunBaseChance = 40,
        RunLowHpBonus = 25,
        RunLowHpThresholdDivisor = 3,
        RunOutnumberedBonus = 15,
        RunMinChance = 10,
        RunMaxChance = 85,
        LootBaseGold = 5,
        LootPowerDivisor = 3,
        LootLevelMultiplier = 4,
        LootVarianceDivisor = 2
    };

    /// <summary>
    /// Default test localization settings with simple, parseable messages.
    /// </summary>
    public static LocalizationSettings CreateTestLocalizationSettings() => new()
    {
        AttackMessages = ["{0} attacks {1} for {2} damage ({3} ? {4})."],
        AttackNothingMessages = ["{0} attacks nothing."],
        AttackCritMessages = ["{0} CRITS {1} for {2} damage ({3} ? {4})!"],
        MagicMessages = ["{0} casts magic on {1} for {2} damage ({3} ? {4})."],
        MagicNoTargetMessages = ["{0} casts magic at nothing."],
        MagicCritMessages = ["{0} CRITS magic on {1} for {2} damage ({3} ? {4})!"],
        TalkNoTargetMessages = ["{0} talks to no one."],
        TalkSuccessMessages = ["{0} convinces {1} to surrender."],
        TalkFailMessages = ["{0} fails to convince {1}."],
        PrayNoTargetMessages = ["{0} prays but no one listens."],
        PraySuccessMessages = ["{0}'s prayer SMITES {1}!"],
        PrayFailMessages = ["{0}'s prayer fails."],
        PrayHealMessages = ["{0} heals self for {1} HP ({2} ? {3})."],
        RunSuccessMessages = ["{0} ESCAPES!"],
        RunFailMessages = ["{0} fails to escape."],
        RunFailOutnumberedMessages = ["{0} can't escape — outnumbered!"],
        MobAttackMessages = ["{0} attacks {1} for {2} damage ({3} ? {4})."],
        DamageReducedMessages = ["{0} reduces damage by {1}."],
        MobKilledMessages = ["{0} kills {1}."],
        PlayerKilledMessages = ["{0} kills {1}."],
        LootMessages = ["Party loots {1} gold from {0}."],
        LootRemainderMessages = ["{0} gold lost."],
        PartyLootSummaryMessages = ["{0} gold split among {1} players."],
        HealMessages = ["{0} heals {1} for {2} HP ({3} ? {4})."],
        HealNoTargetMessages = ["{0} has no one to heal."],
        Locations = ["You are in a dark cave."]
    };

    /// <summary>
    /// Default test bot settings with thematic names.
    /// </summary>
    public static BotSettings CreateTestBotSettings() => new()
    {
        MinimumPartySize = 4,
        StatLevelVariance = 2,
        BaseHp = 20,
        HpPerLevel = 5,
        BasePower = 1,
        PowerPerLevel = 2,
        MinimumBotLevel = 1,
        BotNames =
        [
            "Sir Clanksworth",
            "Mystic Whisperwind",
            "Grimjaw the Bold",
            "Thornweave",
            "Shadowmend",
            "Ironheart"
        ]
    };

    /// <summary>
    /// Default test progression settings for skill-based advancement.
    /// </summary>
    public static ProgressionSettings CreateTestProgressionSettings() => new()
    {
        BaseHp = 20,
        BaseAttackPower = 5,
        BaseMagicPower = 5,
        BaseSpeechPower = 5,
        BaseDefensePower = 2,
        HpPerDefenseLevel = 5,
        AttackPerLevel = 2,
        MagicPerLevel = 2,
        SpeechPerLevel = 2,
        DefensePerLevel = 1,
        BaseSkillXp = 10,
        SkillXpPerDungeonLevel = 2,
        SkillXpPerLevel = 50,
        MaxSkillLevel = 99,
        DefenseXpPerDamage = 0.5
    };

    /// <summary>
    /// Creates a player progression service with test configuration.
    /// </summary>
    public static IPlayerProgressionService CreateProgressionService(ProgressionSettings? settings = null)
    {
        settings ??= CreateTestProgressionSettings();
        return new PlayerProgressionService(Options.Create(settings));
    }
}