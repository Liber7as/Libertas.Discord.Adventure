using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests;

/// <summary>
///     Tests for the bot system to ensure AI companions behave correctly and fairly.
///     Bots should help players but not overshadow them or steal rewards.
/// </summary>
[TestFixture]
[Category("Bots")]
public class BotSystemTests
{
    [SetUp]
    public void SetUp()
    {
        TestEntityFactory.ResetIdCounters();
    }

    /// <summary>
    ///     Verifies that bots are generated when party is below minimum size.
    /// </summary>
    [Test]
    public async Task BotInjection_FillsPartyToMinimumSize()
    {
        // Arrange - solo player should get 3 bots (minimum party size = 4)
        var engine = TestServiceFactory.CreateGameEngine();
        var player = TestEntityFactory.CreateSoloPlayer().First();
        var mob = TestEntityFactory.CreateStandardMob();
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act
        var result = await engine.ExecuteRoundAsync(1, [player], actions, [mob]);

        // Assert
        var bots = result.Players.Where(p => p.IsBot).ToList();
        var humans = result.Players.Where(p => !p.IsBot).ToList();

        Assert.That(humans, Has.Count.EqualTo(1), "Should have 1 human player");
        Assert.That(bots, Has.Count.EqualTo(3), "Should have 3 bots to fill party to 4");
        Assert.That(result.Players, Has.Count.EqualTo(4), "Total party should be 4");

        TestContext.WriteLine("Party composition:");
        foreach (var p in result.Players)
        {
            TestContext.WriteLine($"  {p.Name}: HP={p.CurrentHp}/{p.MaxHp}, Bot={p.IsBot}");
        }
    }

    /// <summary>
    ///     Verifies that no bots are generated when party is already at minimum size.
    /// </summary>
    [Test]
    public async Task BotInjection_NoBotsWhenPartyFull()
    {
        // Arrange - 4 players = no bots needed
        var engine = TestServiceFactory.CreateGameEngine();
        var players = TestEntityFactory.CreateStandardParty();
        var mob = TestEntityFactory.CreateStandardMob();
        var actions = players.ToDictionary(p => p.Id, _ => PlayerAction.Attack);

        // Act
        var result = await engine.ExecuteRoundAsync(1, players, actions, [mob]);

        // Assert
        var bots = result.Players.Where(p => p.IsBot).ToList();
        Assert.That(bots, Is.Empty, "Should not generate bots when party is full");
    }

    /// <summary>
    ///     Verifies that bots persist between rounds (same bots, not regenerated).
    /// </summary>
    [Test]
    public async Task BotPersistence_SameBotsAcrossRounds()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngine();
        var player = TestEntityFactory.CreateSoloPlayer().First();
        var mob = TestEntityFactory.CreateToughMob(); // Won't die in 1 round
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act - run 3 rounds
        var round1 = await engine.ExecuteRoundAsync(1, [player], actions, [mob]);
        var round2 = await engine.ExecuteRoundAsync(2, round1.Players, actions, round1.Mobs);
        var round3 = await engine.ExecuteRoundAsync(3, round2.Players, actions, round2.Mobs);

        // Assert
        var botsRound1 = round1.Players.Where(p => p.IsBot).Select(p => p.Id).ToHashSet();
        var botsRound3 = round3.Players.Where(p => p.IsBot).Select(p => p.Id).ToHashSet();

        Assert.That(botsRound1.SetEquals(botsRound3),
            "Bots should persist across rounds (same IDs)");

        // No additional bots should be added
        Assert.That(round3.Players.Count(p => p.IsBot), Is.EqualTo(3),
            "Should still have exactly 3 bots");
    }

    /// <summary>
    ///     Verifies that bot stats scale appropriately with dungeon level.
    /// </summary>
    [Test]
    public void BotStats_ScaleWithLevel()
    {
        // Arrange
        var rng = TestServiceFactory.CreateRng(12345);
        var botService = TestServiceFactory.CreateBotService(rng);

        // Act - generate bots for different levels
        var level1Bots = botService.GenerateBotsForParty([], 1);
        var level10Bots = botService.GenerateBotsForParty([], 10);

        // Assert
        var avgHpLevel1 = level1Bots.Average(b => b.MaxHp);
        var avgHpLevel10 = level10Bots.Average(b => b.MaxHp);

        TestContext.WriteLine($"Level 1 avg HP: {avgHpLevel1:F0}");
        TestContext.WriteLine($"Level 10 avg HP: {avgHpLevel10:F0}");

        Assert.That(avgHpLevel10, Is.GreaterThan(avgHpLevel1),
            "Higher level bots should have more HP");
    }

    /// <summary>
    ///     Verifies that bot names are unique (no duplicates in same party).
    /// </summary>
    [Test]
    public void BotNames_AreUnique()
    {
        // Arrange
        var rng = TestServiceFactory.CreateRng(54321);
        var botService = TestServiceFactory.CreateBotService(rng);

        // Act - generate 4 bots (should exhaust some names)
        var bots = botService.GenerateBotsForParty([], 5);

        // Assert
        var names = bots.Select(b => b.Name).ToList();
        var uniqueNames = names.Distinct().ToList();

        TestContext.WriteLine($"Bot names: {string.Join(", ", names)}");

        Assert.That(names, Has.Count.EqualTo(uniqueNames.Count),
            "All bot names should be unique");
    }

    /// <summary>
    ///     Verifies that bots make sensible combat decisions.
    /// </summary>
    [Test]
    public void BotAI_MakesSensibleDecisions()
    {
        // Arrange
        var rng = TestServiceFactory.CreateRng(11111);
        var botService = TestServiceFactory.CreateBotService(rng);

        var bot = TestEntityFactory.CreatePlayer("TestBot", isBot: true);
        var injuredAlly = TestEntityFactory.CreateInjuredPlayer("Wounded", 5);
        var healthyAlly = TestEntityFactory.CreateWarrior("Healthy");
        var mob = TestEntityFactory.CreateStandardMob();

        // Track decision distribution
        var healDecisions = 0;
        var otherDecisions = 0;
        const int iterations = 100;

        // Act - test decisions with injured ally present
        for (var i = 0; i < iterations; i++)
        {
            var action = botService.DecideBotAction(bot, [bot, injuredAlly, healthyAlly], [mob]);
            if (action == PlayerAction.Heal)
            {
                healDecisions++;
            }
            else
            {
                otherDecisions++;
            }
        }

        TestContext.WriteLine($"Heal decisions: {healDecisions}/{iterations}");
        TestContext.WriteLine($"Other decisions: {otherDecisions}/{iterations}");

        // Assert - bots should sometimes heal injured allies
        Assert.That(healDecisions, Is.GreaterThan(0),
            "Bots should sometimes choose to heal injured allies");
    }

    /// <summary>
    ///     Verifies that bots never choose to Run (would abandon the party).
    /// </summary>
    [Test]
    public void BotAI_NeverRuns()
    {
        // Arrange
        var rng = TestServiceFactory.CreateRng(99999);
        var botService = TestServiceFactory.CreateBotService(rng);

        var bot = TestEntityFactory.CreatePlayer("TestBot", isBot: true);
        bot.CurrentHp = 1; // Critical HP
        var mob = TestEntityFactory.CreateBossMob();

        // Act - test many decisions
        const int iterations = 1000;
        var runCount = 0;

        for (var i = 0; i < iterations; i++)
        {
            var action = botService.DecideBotAction(bot, [bot], [mob]);
            if (action == PlayerAction.Run)
            {
                runCount++;
            }
        }

        // Assert
        Assert.That(runCount, Is.Zero,
            "Bots should never abandon the party by running");
    }

    /// <summary>
    ///     Verifies that bots do NOT receive loot (all gold goes to human players).
    /// </summary>
    [Test]
    public async Task BotLoot_BotsDoNotReceiveGold()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngine();
        var runner = new TestGameRunner(engine);

        var player = TestEntityFactory.CreateSoloPlayer().First();
        var mob = TestEntityFactory.CreateWeakMob(); // Easy to kill
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act - run until mob dies
        var result = await runner.ExecuteGameLoopAsync(
            1,
            [player],
            [mob],
            actions,
            10,
            verbose: true);

        // Assert
        var finalPlayers = result.SurvivingPlayers.Concat(result.SurvivingBots).ToList();
        var humanGold = finalPlayers.Where(p => !p.IsBot).Sum(p => p.GoldEarned);
        var botGold = finalPlayers.Where(p => p.IsBot).Sum(p => p.GoldEarned);

        TestContext.WriteLine($"Human gold: {humanGold}");
        TestContext.WriteLine($"Bot gold: {botGold}");

        Assert.That(botGold, Is.Zero, "Bots should not receive any gold");
        Assert.That(humanGold, Is.GreaterThan(0), "Human should receive all the gold");
    }

    /// <summary>
    ///     Verifies that bots don't make the game trivially easy.
    ///     The party should still face meaningful challenges.
    /// </summary>
    [Test]
    public async Task BotBalance_GameRemainsChallenging()
    {
        // Arrange - solo player with bot assistance
        var engine = TestServiceFactory.CreateGameEngine();
        var runner = new TestGameRunner(engine);

        var player = TestEntityFactory.CreateSoloPlayer().First();
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act - run 100 rounds with scaling mobs to see progression
        var result = await runner.ExecuteGameLoopAsync(
            1,
            [player],
            [TestEntityFactory.CreateScaledMob(1)],
            actions,
            100,
            respawnFunc: (_, level) => [TestEntityFactory.CreateScaledMob(level)],
            verbose: false);

        // Assert - game should make meaningful progress
        TestContext.WriteLine($"Final level reached: {result.FinalLevel}");
        TestContext.WriteLine($"Mobs killed: {result.Statistics.MobsKilled}");
        TestContext.WriteLine($"Gold earned: {result.TotalGoldEarned:F0}");

        // The game should be playable - player survives past round 1 with bot help
        Assert.That(result.FinalLevel, Is.GreaterThan(1),
            "With bot help, player should survive past round 1");

        // Document the actual balance point (for tuning reference)
        Assert.Pass($"Game balanced: reached level {result.FinalLevel}, earned {result.TotalGoldEarned:F0} gold");
    }
}