using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests;

/// <summary>
/// Visual simulation tests that output detailed game runs for manual verification.
/// These tests are designed to produce readable output that can be reviewed
/// to ensure the game "feels right" and behaves as expected.
/// 
/// Run these tests with verbose output to see the full game narrative.
/// </summary>
[TestFixture]
[Category("Simulation")]
public class GameSimulationTests
{
    [SetUp]
    public void SetUp()
    {
        TestEntityFactory.ResetIdCounters();
    }

    /// <summary>
    /// Simulates a solo adventure with bot companions.
    /// Visual output shows how bots assist the player through combat.
    /// </summary>
    [Test]
    public async Task Simulation_SoloAdventure_WithBotCompanions()
    {
        // Arrange (use seeded RNG for deterministic simulation)
        var rng = TestServiceFactory.CreateRng(42);
        var engine = TestServiceFactory.CreateGameEngine(rng);
        var runner = new TestGameRunner(engine);

        var hero = TestEntityFactory.CreatePlayer(
            "Brave Hero",
            maxHp: 40,
            attackPower: 10,
            magicPower: 8,
            speechPower: 6,
            defensePower: 4);

        var actions = new Dictionary<PlayerId, PlayerAction>
        {
            [hero.Id] = PlayerAction.Attack
        };

        TestContext.WriteLine("\n?? SOLO ADVENTURE WITH BOT COMPANIONS\n");
        TestContext.WriteLine("A lone hero ventures into the dungeon...");
        TestContext.WriteLine("Fortunately, AI companions will join to help!\n");

        // Act
        var result = await runner.ExecuteGameLoopAsync(
            startLevel: 1,
            players: [hero],
            mobs: [TestEntityFactory.CreateWeakMob("Goblin Scout")],
            actions: actions,
            maxRounds: 30,
            respawnFunc: (_, level) =>
            {
                // Progressive difficulty
                return level switch
                {
                    < 5 => [TestEntityFactory.CreateWeakMob($"Goblin Warrior")],
                    < 10 => [TestEntityFactory.CreateStandardMob($"Orc Grunt")],
                    < 15 => [TestEntityFactory.CreateToughMob($"Troll Berserker")],
                    _ => [TestEntityFactory.CreateBossMob($"Dungeon Lord")]
                };
            },
            verbose: true);

        // Summary
        TestContext.WriteLine("\n?? ADVENTURE SUMMARY");
        TestContext.WriteLine($"  Final Level: {result.FinalLevel}");
        TestContext.WriteLine($"  Gold Earned: {result.TotalGoldEarned:F0}");
        TestContext.WriteLine($"  Mobs Slain: {result.Statistics.MobsKilled}");

        Assert.Pass("Visual simulation completed - review output for behavior verification");
    }

    /// <summary>
    /// Simulates a full party adventure with varied actions.
    /// Each player uses a different strategy.
    /// </summary>
    [Test]
    public async Task Simulation_FullParty_VariedStrategies()
    {
        // Arrange (use seeded RNG for deterministic simulation)
        var rng = TestServiceFactory.CreateRng(42);
        var engine = TestServiceFactory.CreateGameEngineWithoutBots(rng);
        var runner = new TestGameRunner(engine);

        var players = new List<PlayerState>
        {
            TestEntityFactory.CreateWarrior("Thorin"),    // Attacks
            TestEntityFactory.CreateMage("Gandalf"),      // Magic
            TestEntityFactory.CreateCleric("Elrond"),     // Heals
            TestEntityFactory.CreateRogue("Bilbo")        // Talks (tries diplomacy)
        };

        // Each player has a preferred action
        var actions = new Dictionary<PlayerId, PlayerAction>
        {
            [players[0].Id] = PlayerAction.Attack,
            [players[1].Id] = PlayerAction.Magic,
            [players[2].Id] = PlayerAction.Heal,
            [players[3].Id] = PlayerAction.Talk
        };

        TestContext.WriteLine("\n?? FELLOWSHIP ADVENTURE\n");
        TestContext.WriteLine("The party composition:");
        TestContext.WriteLine("  ??  Thorin (Warrior) - Attacks");
        TestContext.WriteLine("  ?? Gandalf (Mage) - Magic");
        TestContext.WriteLine("  ?? Elrond (Cleric) - Heals");
        TestContext.WriteLine("  ???  Bilbo (Rogue) - Diplomacy\n");

        // Act
        var result = await runner.ExecuteGameLoopAsync(
            startLevel: 1,
            players: players,
            mobs: [TestEntityFactory.CreateStandardMob("Cave Troll")],
            actions: actions,
            maxRounds: 25,
            respawnFunc: (_, level) =>
            {
                var names = new[] { "Mountain Orc", "Warg Rider", "Goblin King", "Balrog" };
                var name = names[Math.Min(level / 5, names.Length - 1)];
                return [TestEntityFactory.CreateScaledMob(level, name)];
            },
            verbose: true);

        TestContext.WriteLine("\n?? FELLOWSHIP SUMMARY");
        TestContext.WriteLine($"  Rounds Survived: {result.FinalLevel}");
        TestContext.WriteLine($"  Total Gold: {result.TotalGoldEarned:F0}");
        TestContext.WriteLine($"  Divine Interventions: {result.Statistics.DivineSmites}");
        TestContext.WriteLine($"  Heals Performed: {result.Statistics.HealsPerformed}");

        Assert.Pass("Visual simulation completed - review output for behavior verification");
    }

    /// <summary>
    /// Simulates random actions to show diverse game outcomes.
    /// Useful for verifying all action types work in combination.
    /// </summary>
    [Test]
    public async Task Simulation_RandomActions_ShowsDiversity()
    {
        // Arrange (use seeded RNG for deterministic simulation)
        var rng = TestServiceFactory.CreateRng(42);
        var engine = TestServiceFactory.CreateGameEngineWithoutBots(rng);
        var runner = new TestGameRunner(engine);

        var players = new List<PlayerState>
        {
            TestEntityFactory.CreatePlayer("Alice", attackPower: 8, magicPower: 8, speechPower: 8),
            TestEntityFactory.CreatePlayer("Bob", attackPower: 8, magicPower: 8, speechPower: 8),
            TestEntityFactory.CreatePlayer("Carol", attackPower: 8, magicPower: 8, speechPower: 8),
            TestEntityFactory.CreatePlayer("Dave", attackPower: 8, magicPower: 8, speechPower: 8)
        };

        var allActions = new[] { PlayerAction.Attack, PlayerAction.Magic, PlayerAction.Heal, PlayerAction.Talk, PlayerAction.Pray };

        TestContext.WriteLine("\n?? RANDOM ACTION ADVENTURE\n");
        TestContext.WriteLine("Players will choose random actions each round...\n");

        // Act
        var result = await runner.ExecuteGameLoopAsync(
            startLevel: 1,
            players: players,
            mobs: [TestEntityFactory.CreateStandardMob("Wild Monster")],
            actions: players.ToDictionary(p => p.Id, _ => PlayerAction.Attack), // Fallback
            actionChooser: (level, ps, _) =>
            {
                return ps.Where(p => !p.IsBot && p.IsAlive)
                    .ToDictionary(
                        p => p.Id,
                        _ => allActions[rng.Next(0, allActions.Length)]);
            },
            maxRounds: 30,
            respawnFunc: (_, level) => [TestEntityFactory.CreateScaledMob(level)],
            verbose: true);

        TestContext.WriteLine("\n?? CHAOS SUMMARY");
        TestContext.WriteLine($"  Rounds: {result.FinalLevel}");
        TestContext.WriteLine($"  Crits: {result.Statistics.CriticalHits}");
        TestContext.WriteLine($"  Smites: {result.Statistics.DivineSmites}");
        TestContext.WriteLine($"  Escapes: {result.Statistics.SuccessfulEscapes}");
        TestContext.WriteLine($"  Heals: {result.Statistics.HealsPerformed}");

        Assert.Pass("Visual simulation completed - review output for behavior verification");
    }

    /// <summary>
    /// Simulates an endurance run to see how far a party can progress.
    /// Tests scaling and survivability over many rounds.
    /// </summary>
    [Test]
    public async Task Simulation_EnduranceRun_HowFarCanWeGo()
    {
        // Arrange (use seeded RNG for deterministic simulation)
        var rng = TestServiceFactory.CreateRng(42);
        var engine = TestServiceFactory.CreateGameEngine(rng);
        var runner = new TestGameRunner(engine);

        // Create an optimal party
        var players = new List<PlayerState>
        {
            TestEntityFactory.CreateTank("Aegis"),
            TestEntityFactory.CreateCleric("Mercy")
        };

        var actions = new Dictionary<PlayerId, PlayerAction>
        {
            [players[0].Id] = PlayerAction.Attack,
            [players[1].Id] = PlayerAction.Heal
        };

        TestContext.WriteLine("\n?? ENDURANCE CHALLENGE\n");
        TestContext.WriteLine("Tank + Healer composition with bot support...");
        TestContext.WriteLine("How far can they go?\n");

        // Act
        var result = await runner.ExecuteGameLoopAsync(
            startLevel: 1,
            players: players,
            mobs: [TestEntityFactory.CreateWeakMob()],
            actions: actions,
            maxRounds: 100,
            respawnFunc: (_, level) => [TestEntityFactory.CreateScaledMob(level)],
            verbose: true);

        TestContext.WriteLine("\n?? ENDURANCE RESULTS");
        TestContext.WriteLine($"  Final Level: {result.FinalLevel}");
        TestContext.WriteLine($"  Total Gold: {result.TotalGoldEarned:F0}");
        TestContext.WriteLine($"  Mobs Defeated: {result.Statistics.MobsKilled}");
        TestContext.WriteLine($"  Total Heals: {result.Statistics.HealsPerformed}");

        // Document the typical progression range
        Assert.That(result.FinalLevel, Is.GreaterThan(1),
            "Party should survive past level 1");

        Assert.Pass($"Endurance run completed - reached level {result.FinalLevel}");
    }

    /// <summary>
    /// Simulates a boss fight scenario.
    /// Tests party coordination against a powerful single enemy.
    /// </summary>
    [Test]
    public async Task Simulation_BossFight_CoordinatedAssault()
    {
        // Arrange (use seeded RNG for deterministic simulation)
        var rng = TestServiceFactory.CreateRng(42);
        var engine = TestServiceFactory.CreateGameEngineWithoutBots(rng);
        var runner = new TestGameRunner(engine);

        var dps1 = TestEntityFactory.CreateWarrior("DPS1");
        dps1.AttackPower = new PowerLevel(15);
        var dps2 = TestEntityFactory.CreateWarrior("DPS2");
        dps2.AttackPower = new PowerLevel(15);
        var healer1 = TestEntityFactory.CreateCleric("Healer1");
        healer1.MagicPower = new PowerLevel(20);
        var healer2 = TestEntityFactory.CreateCleric("Healer2");
        healer2.MagicPower = new PowerLevel(20);

        var players = new List<PlayerState> { dps1, dps2, healer1, healer2 };

        var actions = new Dictionary<PlayerId, PlayerAction>
        {
            [players[0].Id] = PlayerAction.Attack,
            [players[1].Id] = PlayerAction.Attack,
            [players[2].Id] = PlayerAction.Heal,
            [players[3].Id] = PlayerAction.Heal
        };

        TestContext.WriteLine("\n?? BOSS FIGHT: THE DRAGON\n");
        TestContext.WriteLine("Party: 2 DPS, 2 Healers");
        TestContext.WriteLine("Strategy: All-out attack with healing support\n");

        // Act
        var boss = TestEntityFactory.CreateBossMob("Ancient Dragon");
        boss.AttackPower = new PowerLevel(15); // Slightly nerfed for fair fight

        var result = await runner.ExecuteGameLoopAsync(
            startLevel: 10,
            players: players,
            mobs: [boss],
            actions: actions,
            maxRounds: 50,
            verbose: true);

        var victory = result.SurvivingPlayers.Count > 0 && result.Statistics.MobsKilled > 0;

        TestContext.WriteLine($"\n?? BATTLE RESULT: {(victory ? "VICTORY!" : "DEFEAT")}");
        TestContext.WriteLine($"  Survivors: {result.SurvivingPlayers.Count}/4");
        TestContext.WriteLine($"  Rounds: {result.FinalLevel - 9}");

        Assert.Pass($"Boss fight completed - {(victory ? "Victory" : "Defeat")}");
    }
}
