using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Core.Settings;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests;

/// <summary>
///     Tests for edge cases and boundary conditions.
///     These tests ensure the game handles unusual situations gracefully.
/// </summary>
[TestFixture]
[Category("EdgeCases")]
public class EdgeCaseTests
{
    [SetUp]
    public void SetUp()
    {
        TestEntityFactory.ResetIdCounters();
    }

    /// <summary>
    ///     Verifies the game handles a round with no players gracefully.
    /// </summary>
    [Test]
    public async Task NoPlayers_HandledGracefully()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var mob = TestEntityFactory.CreateStandardMob();
        var actions = new Dictionary<PlayerId, PlayerAction>();

        // Act
        var result = await engine.ExecuteRoundAsync(1, [], actions, [mob]);

        // Assert
        Assert.That(result.Players, Is.Empty, "Should have no players");
        Assert.That(result.Mobs.First().CurrentHp, Is.EqualTo(mob.CurrentHp),
            "Mob should be undamaged with no players");
    }

    /// <summary>
    ///     Verifies the game handles a round with no mobs gracefully.
    /// </summary>
    [Test]
    public async Task NoMobs_HandledGracefully()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var player = TestEntityFactory.CreateWarrior();
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act
        var result = await engine.ExecuteRoundAsync(1, [player], actions, []);

        // Assert
        Assert.That(result.Mobs, Is.Empty, "Should have no mobs");
        Assert.That(result.Players.First().CurrentHp, Is.EqualTo(player.CurrentHp),
            "Player should be undamaged with no mobs");
    }

    /// <summary>
    ///     Verifies handling when all players are already dead.
    /// </summary>
    [Test]
    public async Task AllPlayersDead_NoActionsProcessed()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var deadPlayer = TestEntityFactory.CreatePlayer("Ghost", currentHp: 0);
        var mob = TestEntityFactory.CreateStandardMob();
        var actions = new Dictionary<PlayerId, PlayerAction> { [deadPlayer.Id] = PlayerAction.Attack };

        // Act
        var result = await engine.ExecuteRoundAsync(1, [deadPlayer], actions, [mob]);

        // Assert
        Assert.That(result.Mobs.First().CurrentHp, Is.EqualTo(mob.CurrentHp),
            "Dead players should not deal damage");
    }

    /// <summary>
    ///     Verifies handling when all mobs are already dead.
    /// </summary>
    [Test]
    public async Task AllMobsDead_NoMobActionsProcessed()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var player = TestEntityFactory.CreateWarrior();
        var deadMob = TestEntityFactory.CreateMob("Corpse", currentHp: 0);
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act
        var result = await engine.ExecuteRoundAsync(1, [player], actions, [deadMob]);

        // Assert
        Assert.That(result.Players.First().CurrentHp, Is.EqualTo(player.CurrentHp),
            "Dead mobs should not deal damage");
    }

    /// <summary>
    ///     Verifies the game handles extremely high stats without overflow.
    /// </summary>
    [Test]
    public async Task ExtremelyHighStats_NoOverflow()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var superPlayer = TestEntityFactory.CreatePlayer(
            "Demigod",
            1000000,
            1000000,
            10000,
            10000,
            10000,
            10000);

        var superMob = TestEntityFactory.CreateMob("Titan", 1000000, 1000000, 10000);
        var originalMobHp = superMob.CurrentHp; // Save before combat (mutable class)
        var actions = new Dictionary<PlayerId, PlayerAction> { [superPlayer.Id] = PlayerAction.Attack };

        // Act & Assert - should not throw
        var result = await engine.ExecuteRoundAsync(1, [superPlayer], actions, [superMob]);

        Assert.That(result.Mobs.First().CurrentHp, Is.LessThan(originalMobHp),
            "Attack should still deal damage with extreme stats");
    }

    /// <summary>
    ///     Verifies the game handles minimum stats (1 power, 1 HP).
    /// </summary>
    [Test]
    public async Task MinimumStats_GameStillFunctions()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var weakPlayer = TestEntityFactory.CreatePlayer(
            "Peasant",
            1,
            1,
            1,
            1,
            1,
            0);

        var weakMob = TestEntityFactory.CreateMob("Rat", 1, 1, 1);
        var actions = new Dictionary<PlayerId, PlayerAction> { [weakPlayer.Id] = PlayerAction.Attack };

        // Act
        var result = await engine.ExecuteRoundAsync(1, [weakPlayer], actions, [weakMob]);

        // Assert - game should function
        Assert.That(result.Messages.Count, Is.GreaterThan(0),
            "Game should produce messages with minimum stats");
    }

    /// <summary>
    ///     Verifies very high level scaling doesn't break the game.
    /// </summary>
    [Test]
    public async Task VeryHighLevel_ScalingDoesNotBreak()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var player = TestEntityFactory.CreateWarrior();
        var mob = TestEntityFactory.CreateStandardMob();
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act - level 100 should still work
        var result = await engine.ExecuteRoundAsync(100, [player], actions, [mob]);

        // Assert
        Assert.That(result.Messages.Count, Is.GreaterThan(0),
            "Game should function at high levels");

        TestContext.WriteLine($"Level 100 round executed with {result.Messages.Count} messages");
    }

    /// <summary>
    ///     Verifies that a player with no action specified defaults to Run.
    /// </summary>
    [Test]
    public async Task MissingAction_DefaultsToRun()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var player = TestEntityFactory.CreateWarrior();
        var mob = TestEntityFactory.CreateStandardMob();
        var actions = new Dictionary<PlayerId, PlayerAction>(); // Empty!

        // Act
        var result = await engine.ExecuteRoundAsync(1, [player], actions, [mob]);

        // Assert - should see run attempt message
        var hasRunMessage = result.Messages.Any(m =>
            m.Contains("escape", StringComparison.OrdinalIgnoreCase) ||
            m.Contains("fails to escape", StringComparison.OrdinalIgnoreCase));

        Assert.That(hasRunMessage, Is.True,
            "Missing action should default to Run");
    }

    /// <summary>
    ///     Verifies heal with only self (no other targets) reports no target.
    /// </summary>
    [Test]
    public async Task HealSelf_WhenOnlyPlayerAndFullHp_ReportsNoTarget()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var player = TestEntityFactory.CreateCleric();
        var mob = TestEntityFactory.CreateMob("Passive", attackPower: 0);
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Heal };

        // Act
        var result = await engine.ExecuteRoundAsync(1, [player], actions, [mob]);

        // Assert
        Assert.That(result.Messages.Any(m => m.Contains("no one to heal")),
            "Should report no healing target when alone and full HP");
    }

    /// <summary>
    ///     Verifies that attacking with no mobs alive does nothing harmful.
    /// </summary>
    [Test]
    public async Task AttackNoTarget_DoesNothing()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var player = TestEntityFactory.CreateWarrior();
        var deadMob = TestEntityFactory.CreateMob("Corpse", currentHp: 0);
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act
        var result = await engine.ExecuteRoundAsync(1, [player], actions, [deadMob]);

        // Assert - game should not crash
        Assert.That(result.Messages.Count, Is.GreaterThan(0));
    }

    /// <summary>
    ///     Verifies bot generation when all names are exhausted.
    /// </summary>
    [Test]
    public void BotNames_FallbackWhenExhausted()
    {
        // Arrange - settings with very few names
        var settings = new BotSettings
        {
            MinimumPartySize = 10,
            BotNames = ["OnlyName"]
        };

        var rng = TestServiceFactory.CreateRng(12345);
        var botService = new BotService(rng, Options.Create(settings), NullLogger<BotService>.Instance);

        // Act - need 10 bots but only 1 name
        var bots = botService.GenerateBotsForParty([], 5);

        // Assert - should not crash, should have fallback names
        Assert.That(bots, Has.Count.EqualTo(10));
        Assert.That(bots.Select(b => b.Name).Distinct().Count(), Is.EqualTo(10),
            "All bots should have unique names (fallback for duplicates)");

        TestContext.WriteLine("Bot names generated:");
        foreach (var bot in bots)
        {
            TestContext.WriteLine($"  {bot.Name}");
        }
    }

    /// <summary>
    ///     Verifies bot level calculation doesn't go negative.
    /// </summary>
    [Test]
    public void BotLevel_NeverNegative()
    {
        // Arrange
        var settings = new BotSettings
        {
            MinimumPartySize = 4,
            StatLevelVariance = 10, // High variance
            MinimumBotLevel = 1,
            BaseHp = 20,
            HpPerLevel = 5,
            BasePower = 1,
            PowerPerLevel = 2,
            BotNames = ["Bot1", "Bot2", "Bot3", "Bot4"]
        };

        var rng = TestServiceFactory.CreateRng(99999);
        var botService = new BotService(rng, Options.Create(settings), NullLogger<BotService>.Instance);

        // Act - level 1 with high variance could try to go negative
        var bots = botService.GenerateBotsForParty([], 1);

        // Assert - all bots should have valid positive stats
        foreach (var bot in bots)
        {
            Assert.That(bot.MaxHp, Is.GreaterThan(0), $"{bot.Name} should have positive HP");
            Assert.That(bot.AttackPower.Value, Is.GreaterThan(0), $"{bot.Name} should have positive attack");
        }
    }

    /// <summary>
    ///     Verifies game handles multiple mobs correctly.
    /// </summary>
    [Test]
    public async Task MultipleMobs_AllCanAttack()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var player = TestEntityFactory.CreateTank();
        var mobs = TestEntityFactory.CreateMobSwarm(5);
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Heal };

        // Act
        var result = await engine.ExecuteRoundAsync(1, [player], actions, mobs);

        // Assert - all mobs should have attacked
        var mobAttackMessages = result.Messages.Count(m =>
            m.Contains("attacks", StringComparison.OrdinalIgnoreCase));

        Assert.That(mobAttackMessages, Is.EqualTo(5),
            "All 5 mobs should attack");

        TestContext.WriteLine($"Player took damage from {mobAttackMessages} mob attacks");
    }

    /// <summary>
    ///     Verifies player attacks only hit one mob at a time.
    /// </summary>
    [Test]
    public async Task PlayerAttack_HitsOnlyOneMob()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var player = TestEntityFactory.CreateWarrior();
        var mobs = TestEntityFactory.CreateMobSwarm();
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act
        var result = await engine.ExecuteRoundAsync(1, [player], actions, mobs);

        // Assert - only one mob should be damaged
        var damagedMobs = result.Mobs.Count(m => m.CurrentHp < 15); // Original HP was 15
        Assert.That(damagedMobs, Is.EqualTo(1),
            "Attack should only hit one mob");
    }
}