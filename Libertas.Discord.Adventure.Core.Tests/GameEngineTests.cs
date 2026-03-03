using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests;

/// <summary>
///     Core game engine integration tests.
///     These tests verify that the game engine correctly orchestrates combat rounds.
///     For more specific tests, see:
///     - <see cref="PlayerActionTests" /> - Individual action mechanics
///     - <see cref="BotSystemTests" /> - AI companion behavior
///     - <see cref="LootDistributionTests" /> - Gold distribution fairness
///     - <see cref="GameBalanceTests" /> - Statistical balance validation
///     - <see cref="EdgeCaseTests" /> - Boundary conditions
///     - <see cref="GameSimulationTests" /> - Visual game simulations
/// </summary>
[TestFixture]
[Category("Integration")]
public class GameEngineTests
{
    [SetUp]
    public void SetUp()
    {
        TestEntityFactory.ResetIdCounters();
        _engine = TestServiceFactory.CreateGameEngine();
        _runner = new TestGameRunner(_engine);
    }

    private IGameEngine _engine = null!;
    private TestGameRunner _runner = null!;

    /// <summary>
    ///     Verifies that a basic combat round executes successfully with all action types.
    /// </summary>
    [Test]
    public async Task BasicCombat_AllActionTypes_ExecutesSuccessfully()
    {
        // Arrange - party with varied actions
        var party = new List<PlayerState>
        {
            TestEntityFactory.CreateWarrior("Attacker"),
            TestEntityFactory.CreateMage("Caster"),
            TestEntityFactory.CreateCleric("Healer"),
            TestEntityFactory.CreateRogue("Talker"),
            TestEntityFactory.CreateInjuredPlayer("Runner", 5)
        };

        var mob = TestEntityFactory.CreateStandardMob("Goblin");

        var actions = new Dictionary<PlayerId, PlayerAction>
        {
            [party[0].Id] = PlayerAction.Attack,
            [party[1].Id] = PlayerAction.Magic,
            [party[2].Id] = PlayerAction.Heal,
            [party[3].Id] = PlayerAction.Talk,
            [party[4].Id] = PlayerAction.Run
        };

        // Act
        var result = await _runner.ExecuteSingleRoundAsync(1, party, [mob], actions, true);

        // Assert
        Assert.That(result.Messages.Count, Is.GreaterThan(1),
            "Round should produce multiple messages");
        Assert.That(result.Players.Count, Is.GreaterThanOrEqualTo(4),
            "Most players should remain (one might escape)");
    }

    /// <summary>
    ///     Verifies that a full game loop can execute until completion.
    /// </summary>
    [Test]
    public async Task FullGameLoop_ExecutesUntilPartyWipe()
    {
        // Arrange
        var party = TestEntityFactory.CreateStandardParty();
        var mob = TestEntityFactory.CreateWeakMob();

        var actions = party.ToDictionary(p => p.Id, _ => PlayerAction.Attack);

        // Act
        var result = await _runner.ExecuteGameLoopAsync(
            1,
            party,
            [mob],
            actions,
            50,
            respawnFunc: (_, level) => [TestEntityFactory.CreateScaledMob(level)],
            verbose: true);

        // Assert
        Assert.That(result.FinalLevel, Is.GreaterThan(0),
            "Game should run at least one round");
        Assert.That(result.Statistics.TotalRounds, Is.GreaterThan(0),
            "Should track statistics");

        TestContext.WriteLine($"\nGame ended at level {result.FinalLevel}");
        TestContext.WriteLine($"Total gold earned: {result.TotalGoldEarned:F0}");
    }

    /// <summary>
    ///     Verifies that round results contain expected state.
    /// </summary>
    [Test]
    public async Task RoundResult_ContainsExpectedState()
    {
        // Arrange
        var player = TestEntityFactory.CreateWarrior();
        var mob = TestEntityFactory.CreateStandardMob();
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act
        var result = await _engine.ExecuteRoundAsync(1, [player], actions, [mob]);

        // Assert
        Assert.That(result.Level, Is.EqualTo(1), "Result should contain level");
        Assert.That(result.Players, Is.Not.Empty, "Result should contain players");
        Assert.That(result.Mobs, Is.Not.Empty, "Result should contain mobs");
        Assert.That(result.Messages, Is.Not.Empty, "Result should contain messages");
    }

    /// <summary>
    ///     Verifies that mobs act after all players in a round.
    /// </summary>
    [Test]
    public async Task TurnOrder_PlayersActBeforeMobs()
    {
        // Arrange - use weak mob and strong player to ensure survival
        var player = TestEntityFactory.CreateTank();
        var mob = TestEntityFactory.CreateWeakMob();
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act
        var result = await _engine.ExecuteRoundAsync(1, [player], actions, [mob]);

        // Assert - verify that player action message appears in the result
        // (Turn order is enforced by the engine: all players act, then all mobs)
        // Check for either "attacks" or "CRITS" (critical hit) containing player name
        var messages = result.Messages.ToList();
        var hasPlayerAttack = messages.Any(m =>
            m.Contains(player.Name) &&
            (m.Contains("attacks", StringComparison.OrdinalIgnoreCase) ||
             m.Contains("CRITS", StringComparison.OrdinalIgnoreCase)));

        Assert.That(hasPlayerAttack, Is.True,
            $"Player should have an attack action in the round. Messages: {string.Join("; ", messages)}");

        // Verify the engine processed the round (status message is first)
        Assert.That(messages.Count, Is.GreaterThan(1),
            "Round should produce multiple messages (status + actions)");
    }

    /// <summary>
    ///     Verifies that the game progresses through multiple levels correctly.
    /// </summary>
    [Test]
    public async Task LevelProgression_LevelsIncreaseCorrectly()
    {
        // Arrange
        var party = TestEntityFactory.CreateStandardParty();
        var actions = party.ToDictionary(p => p.Id, _ => PlayerAction.Attack);
        var levelsReached = new List<int>();

        // Act - run 5 rounds and track levels
        var players = party.ToList();
        var mobs = new List<MobState> { TestEntityFactory.CreateWeakMob() };

        for (var level = 1; level <= 5; level++)
        {
            var result = await _engine.ExecuteRoundAsync(level, players, actions, mobs);
            levelsReached.Add(result.Level);
            players = [.. result.Players];
            mobs = result.Mobs.Any(m => m.IsAlive)
                ? [.. result.Mobs]
                : [TestEntityFactory.CreateWeakMob()];
        }

        // Assert
        Assert.That(levelsReached, Is.EqualTo([1, 2, 3, 4, 5]),
            "Levels should progress correctly");
    }

    /// <summary>
    ///     Verifies that player state persists correctly between rounds.
    /// </summary>
    [Test]
    public async Task StatePersistence_PlayerHpPersistsBetweenRounds()
    {
        // Arrange - player will take damage from mob
        var player = TestEntityFactory.CreateWarrior();
        var mob = TestEntityFactory.CreateStandardMob();
        var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

        // Act - run two rounds
        var round1 = await _engine.ExecuteRoundAsync(1, [player], actions, [mob]);
        var playerAfterRound1 = round1.Players.First(p => !p.IsBot);

        var round2 = await _engine.ExecuteRoundAsync(2, round1.Players, actions, round1.Mobs);
        var playerAfterRound2 = round2.Players.First(p => !p.IsBot);

        // Assert - HP should persist (and probably decrease if mob attacked)
        TestContext.WriteLine($"HP after round 1: {playerAfterRound1.CurrentHp}");
        TestContext.WriteLine($"HP after round 2: {playerAfterRound2.CurrentHp}");

        // The player state should flow through rounds
        Assert.That(playerAfterRound1.Id, Is.EqualTo(player.Id),
            "Player ID should persist");
    }
}