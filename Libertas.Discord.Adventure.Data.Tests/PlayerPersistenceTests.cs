using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Data.Tests;

/// <summary>
/// Integration tests for player persistence and progression.
/// Uses in-memory database for isolation and speed.
/// </summary>
[TestFixture]
[Category("Database")]
[Category("Integration")]
public class PlayerPersistenceTests
{
    #region Player Creation Tests

    /// <summary>
    /// Verifies that a new player is created with correct default values.
    /// </summary>
    [Test]
    public async Task NewPlayer_HasCorrectDefaults()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(NewPlayer_HasCorrectDefaults));
        var playerService = factory.CreatePlayerService();

        // Act
        var playerState = await playerService.GetOrCreateAsync(12345UL, "TestUser");

        // Assert - verify default skill levels
        Assert.That(playerState.Skills.AttackLevel, Is.EqualTo(1), "New player should have Attack level 1");
        Assert.That(playerState.Skills.MagicLevel, Is.EqualTo(1), "New player should have Magic level 1");
        Assert.That(playerState.Skills.SpeechLevel, Is.EqualTo(1), "New player should have Speech level 1");
        Assert.That(playerState.Skills.DefenseLevel, Is.EqualTo(1), "New player should have Defense level 1");

        // Assert - verify default stats (from progression settings)
        Assert.That(playerState.MaxHp, Is.EqualTo(20), "New player should have 20 base HP");
        Assert.That(playerState.AttackPower.Value, Is.EqualTo(5), "New player should have 5 base attack");
        Assert.That(playerState.MagicPower.Value, Is.EqualTo(5), "New player should have 5 base magic");
        Assert.That(playerState.SpeechPower.Value, Is.EqualTo(5), "New player should have 5 base speech");
        Assert.That(playerState.DefensePower.Value, Is.EqualTo(2), "New player should have 2 base defense");

        // Assert - verify other properties
        Assert.That(playerState.Name, Is.EqualTo("TestUser"));
        Assert.That(playerState.Id.Value, Is.EqualTo(12345UL));
        Assert.That(playerState.IsBot, Is.False);

        TestContext.Out.WriteLine($"Created player: {playerState.Name} (ID: {playerState.Id.Value})");
        TestContext.Out.WriteLine($"  Combat Level: {playerState.CombatLevel}");
        TestContext.Out.WriteLine($"  Stats: HP={playerState.MaxHp}, ATK={playerState.AttackPower.Value}, MAG={playerState.MagicPower.Value}");
    }

    /// <summary>
    /// Verifies that getting the same player twice returns consistent data.
    /// </summary>
    [Test]
    public async Task GetPlayer_ReturnsSameData()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(GetPlayer_ReturnsSameData));
        var playerService = factory.CreatePlayerService();

        // Act
        var firstGet = await playerService.GetOrCreateAsync(99999UL, "SameUser");
        var secondGet = await playerService.GetOrCreateAsync(99999UL, "SameUser");

        // Assert
        Assert.That(secondGet.Id.Value, Is.EqualTo(firstGet.Id.Value));
        Assert.That(secondGet.Name, Is.EqualTo(firstGet.Name));
        Assert.That(secondGet.MaxHp, Is.EqualTo(firstGet.MaxHp));
    }

    /// <summary>
    /// Verifies that username updates are persisted.
    /// </summary>
    [Test]
    public async Task UsernameChange_IsPersisted()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(UsernameChange_IsPersisted));
        var playerService = factory.CreatePlayerService();

        // Act - create with one name
        await playerService.GetOrCreateAsync(11111UL, "OldName");

        // Get again with new name
        var updated = await playerService.GetOrCreateAsync(11111UL, "NewName");

        // Assert
        Assert.That(updated.Name, Is.EqualTo("NewName"));

        // Verify in database
        var dbPlayer = await playerService.GetPlayerDataAsync(11111UL);
        Assert.That(dbPlayer!.Name, Is.EqualTo("NewName"));
    }

    #endregion

    #region XP and Level-Up Tests

    /// <summary>
    /// Verifies that XP is correctly accumulated across sessions.
    /// </summary>
    [Test]
    public async Task XpAccumulation_PersistsCorrectly()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(XpAccumulation_PersistsCorrectly));
        var playerService = factory.CreatePlayerService();

        // Create player
        var playerState = await playerService.GetOrCreateAsync(22222UL, "XpTester");

        // Simulate combat earnings
        playerState.AttackXpEarned = 25;
        playerState.MagicXpEarned = 15;
        playerState.SpeechXpEarned = 10;
        playerState.DefenseXpEarned = 5;

        // Act - save progress
        await playerService.SaveProgressAsync(playerState, dungeonLevel: 1, died: false);

        // Assert - check database
        var dbPlayer = await playerService.GetPlayerDataAsync(22222UL);
        Assert.That(dbPlayer!.AttackXpTotal, Is.EqualTo(25), "Attack XP should be saved");
        Assert.That(dbPlayer.MagicXpTotal, Is.EqualTo(15), "Magic XP should be saved");
        Assert.That(dbPlayer.SpeechXpTotal, Is.EqualTo(10), "Speech XP should be saved");
        Assert.That(dbPlayer.DefenseXpTotal, Is.EqualTo(5), "Defense XP should be saved");

        TestContext.Out.WriteLine($"Saved XP: ATK={dbPlayer.AttackXpTotal}, MAG={dbPlayer.MagicXpTotal}, SPE={dbPlayer.SpeechXpTotal}, DEF={dbPlayer.DefenseXpTotal}");
    }

    /// <summary>
    /// Verifies that XP from multiple sessions accumulates correctly.
    /// </summary>
    [Test]
    public async Task MultipleSessionXp_AccumulatesCorrectly()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(MultipleSessionXp_AccumulatesCorrectly));
        var playerService = factory.CreatePlayerService();

        // Session 1
        var session1 = await playerService.GetOrCreateAsync(33333UL, "MultiSession");
        session1.AttackXpEarned = 30;
        await playerService.SaveProgressAsync(session1, dungeonLevel: 1, died: false);

        // Session 2 - get fresh state
        var session2 = await playerService.GetOrCreateAsync(33333UL, "MultiSession");
        session2.AttackXpEarned = 20; // New XP this session
        await playerService.SaveProgressAsync(session2, dungeonLevel: 2, died: false);

        // Session 3
        var session3 = await playerService.GetOrCreateAsync(33333UL, "MultiSession");
        session3.AttackXpEarned = 50;
        await playerService.SaveProgressAsync(session3, dungeonLevel: 3, died: false);

        // Assert
        var dbPlayer = await playerService.GetPlayerDataAsync(33333UL);
        Assert.That(dbPlayer!.AttackXpTotal, Is.EqualTo(100), "Total Attack XP should be 30+20+50=100");
        Assert.That(dbPlayer.HighestDungeonLevel, Is.EqualTo(3), "Highest dungeon level should be 3");

        TestContext.Out.WriteLine($"After 3 sessions: Total ATK XP = {dbPlayer.AttackXpTotal}");
    }

    /// <summary>
    /// Verifies that level-ups are correctly detected when XP threshold is crossed.
    /// </summary>
    [Test]
    public async Task LevelUp_DetectedCorrectly()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(LevelUp_DetectedCorrectly));
        var playerService = factory.CreatePlayerService();
        var progressionService = factory.CreateProgressionService();

        // Get XP required for level 2 (should be 50 with default settings)
        var xpForLevel2 = progressionService.GetXpRequiredForSkillLevel(2);

        var playerState = await playerService.GetOrCreateAsync(44444UL, "LevelUpTest");
        playerState.AttackXpEarned = xpForLevel2; // Exactly enough for level 2

        // Act
        var result = await playerService.SaveProgressAsync(playerState, dungeonLevel: 1, died: false);

        // Assert
        Assert.That(result.LevelUps, Has.Count.EqualTo(1), "Should have exactly one level-up");
        Assert.That(result.LevelUps[0].SkillName, Is.EqualTo("Attack"));
        Assert.That(result.LevelUps[0].OldLevel, Is.EqualTo(1));
        Assert.That(result.LevelUps[0].NewLevel, Is.EqualTo(2));

        // Verify in database
        var dbPlayer = await playerService.GetPlayerDataAsync(44444UL);
        Assert.That(dbPlayer!.Skills.AttackLevel, Is.EqualTo(2), "Attack level should be 2 in database");

        TestContext.Out.WriteLine($"Level up detected: {result.LevelUps[0].SkillName} {result.LevelUps[0].OldLevel} -> {result.LevelUps[0].NewLevel}");
    }

    /// <summary>
    /// Verifies that multiple level-ups in one session are all detected.
    /// </summary>
    [Test]
    public async Task MultipleLevelUps_AllDetected()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(MultipleLevelUps_AllDetected));
        var playerService = factory.CreatePlayerService();
        var progressionService = factory.CreateProgressionService();

        var xpForLevel5 = progressionService.GetXpRequiredForSkillLevel(5);

        var playerState = await playerService.GetOrCreateAsync(55555UL, "MultiLevelUp");

        // Grant enough XP to go from level 1 to level 5 in all skills
        playerState.AttackXpEarned = xpForLevel5;
        playerState.MagicXpEarned = xpForLevel5;
        playerState.SpeechXpEarned = xpForLevel5;
        playerState.DefenseXpEarned = xpForLevel5;

        // Act
        var result = await playerService.SaveProgressAsync(playerState, dungeonLevel: 1, died: false);

        // Assert
        Assert.That(result.LevelUps, Has.Count.EqualTo(4), "Should have 4 level-ups (one per skill)");
        Assert.That(result.LevelUps.All(lu => lu.OldLevel == 1 && lu.NewLevel == 5), Is.True,
            "All skills should go from level 1 to 5");

        var dbPlayer = await playerService.GetPlayerDataAsync(55555UL);
        Assert.That(dbPlayer!.TotalLevel, Is.EqualTo(20), "Total level should be 5+5+5+5=20");

        TestContext.Out.WriteLine($"Level-ups: {string.Join(", ", result.LevelUps.Select(lu => $"{lu.SkillName}: {lu.OldLevel}->{lu.NewLevel}"))}");
    }

    /// <summary>
    /// Verifies that stats are recalculated correctly after level-ups.
    /// </summary>
    [Test]
    public async Task StatsRecalculation_AfterLevelUp()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(StatsRecalculation_AfterLevelUp));
        var playerService = factory.CreatePlayerService();
        var progressionService = factory.CreateProgressionService();

        var xpForLevel10 = progressionService.GetXpRequiredForSkillLevel(10);

        // Create player and grant XP
        var playerState = await playerService.GetOrCreateAsync(66666UL, "StatsTest");
        playerState.AttackXpEarned = xpForLevel10;
        playerState.DefenseXpEarned = xpForLevel10;
        await playerService.SaveProgressAsync(playerState, dungeonLevel: 1, died: false);

        // Act - reload player to get recalculated stats
        var reloadedPlayer = await playerService.GetOrCreateAsync(66666UL, "StatsTest");

        // Assert - with default settings:
        // Attack at level 10 = 5 + (10-1)*2 = 23
        // Defense at level 10 = 2 + (10-1)*1 = 11
        // HP at Defense level 10 = 20 + (10-1)*5 = 65
        Assert.That(reloadedPlayer.AttackPower.Value, Is.EqualTo(23), "Attack power should scale with level");
        Assert.That(reloadedPlayer.DefensePower.Value, Is.EqualTo(11), "Defense power should scale with level");
        Assert.That(reloadedPlayer.MaxHp, Is.EqualTo(65), "HP should scale with defense level");

        TestContext.Out.WriteLine($"After level-up: HP={reloadedPlayer.MaxHp}, ATK={reloadedPlayer.AttackPower.Value}, DEF={reloadedPlayer.DefensePower.Value}");
    }

    #endregion

    #region Gold and Statistics Tests

    /// <summary>
    /// Verifies that gold is correctly accumulated.
    /// </summary>
    [Test]
    public async Task GoldAccumulation_PersistsCorrectly()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(GoldAccumulation_PersistsCorrectly));
        var playerService = factory.CreatePlayerService();

        var playerState = await playerService.GetOrCreateAsync(77777UL, "GoldTest");
        playerState.GoldEarned = 150.5;

        // Act
        await playerService.SaveProgressAsync(playerState, dungeonLevel: 5, died: false);

        // Assert
        var dbPlayer = await playerService.GetPlayerDataAsync(77777UL);
        Assert.That(dbPlayer!.TotalGold, Is.EqualTo(150.5).Within(0.01));
    }

    /// <summary>
    /// Verifies that kills are correctly tracked.
    /// </summary>
    [Test]
    public async Task KillTracking_PersistsCorrectly()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(KillTracking_PersistsCorrectly));
        var playerService = factory.CreatePlayerService();

        var playerState = await playerService.GetOrCreateAsync(88888UL, "KillTest");
        playerState.MobsKilled = 7;

        // Act
        await playerService.SaveProgressAsync(playerState, dungeonLevel: 3, died: false);

        // Assert
        var dbPlayer = await playerService.GetPlayerDataAsync(88888UL);
        Assert.That(dbPlayer!.TotalKills, Is.EqualTo(7));
    }

    /// <summary>
    /// Verifies that deaths are correctly tracked.
    /// </summary>
    [Test]
    public async Task DeathTracking_PersistsCorrectly()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(DeathTracking_PersistsCorrectly));
        var playerService = factory.CreatePlayerService();

        var playerState = await playerService.GetOrCreateAsync(99991UL, "DeathTest");

        // Act - die in first session
        await playerService.SaveProgressAsync(playerState, dungeonLevel: 5, died: true);

        // Die again in second session
        var session2 = await playerService.GetOrCreateAsync(99991UL, "DeathTest");
        await playerService.SaveProgressAsync(session2, dungeonLevel: 3, died: true);

        // Assert
        var dbPlayer = await playerService.GetPlayerDataAsync(99991UL);
        Assert.That(dbPlayer!.TotalDeaths, Is.EqualTo(2));
    }

    /// <summary>
    /// Verifies that highest dungeon level only increases, never decreases.
    /// </summary>
    [Test]
    public async Task HighestDungeonLevel_OnlyIncreases()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(HighestDungeonLevel_OnlyIncreases));
        var playerService = factory.CreatePlayerService();

        // Session 1 - reach level 10
        var session1 = await playerService.GetOrCreateAsync(99992UL, "DungeonTest");
        await playerService.SaveProgressAsync(session1, dungeonLevel: 10, died: true);

        // Session 2 - only reach level 5
        var session2 = await playerService.GetOrCreateAsync(99992UL, "DungeonTest");
        await playerService.SaveProgressAsync(session2, dungeonLevel: 5, died: false);

        // Session 3 - reach level 15
        var session3 = await playerService.GetOrCreateAsync(99992UL, "DungeonTest");
        await playerService.SaveProgressAsync(session3, dungeonLevel: 15, died: false);

        // Assert
        var dbPlayer = await playerService.GetPlayerDataAsync(99992UL);
        Assert.That(dbPlayer!.HighestDungeonLevel, Is.EqualTo(15), "Highest should be 15, not 5");
    }

    #endregion

    #region Bot Handling Tests

    /// <summary>
    /// Verifies that bot players don't have progress saved.
    /// </summary>
    [Test]
    public async Task BotProgress_NotSaved()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(BotProgress_NotSaved));
        var playerService = factory.CreatePlayerService();

        var botState = new PlayerState
        {
            Id = new PlayerId(ulong.MaxValue),
            Name = "TestBot",
            IsBot = true,
            AttackXpEarned = 1000,
            GoldEarned = 999
        };

        // Act
        var result = await playerService.SaveProgressAsync(botState, dungeonLevel: 10, died: false);

        // Assert - no level-ups returned for bots
        Assert.That(result.LevelUps, Is.Empty);

        // Bot should not exist in database
        var dbPlayer = await playerService.GetPlayerDataAsync(ulong.MaxValue);
        Assert.That(dbPlayer, Is.Null, "Bot should not be saved to database");
    }

    #endregion

    #region Edge Cases

    /// <summary>
    /// Verifies that saving progress for non-existent player is handled gracefully.
    /// </summary>
    [Test]
    public async Task SaveProgress_NonExistentPlayer_HandledGracefully()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(SaveProgress_NonExistentPlayer_HandledGracefully));
        var playerService = factory.CreatePlayerService();

        var fakePlayerState = new PlayerState
        {
            Id = new PlayerId(999999999UL),
            Name = "FakePlayer",
            AttackXpEarned = 100
        };

        // Act - should not throw
        var result = await playerService.SaveProgressAsync(fakePlayerState, dungeonLevel: 1, died: false);

        // Assert
        Assert.That(result.LevelUps, Is.Empty, "Should return empty level-ups for non-existent player");
    }

    /// <summary>
    /// Verifies XP does not overflow with very large values.
    /// </summary>
    [Test]
    public async Task LargeXpValues_NoOverflow()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(LargeXpValues_NoOverflow));
        var playerService = factory.CreatePlayerService();

        var playerState = await playerService.GetOrCreateAsync(99993UL, "OverflowTest");
        playerState.AttackXpEarned = 1_000_000_000L;

        // Act
        await playerService.SaveProgressAsync(playerState, dungeonLevel: 1, died: false);

        // Assert
        var dbPlayer = await playerService.GetPlayerDataAsync(99993UL);
        Assert.That(dbPlayer!.AttackXpTotal, Is.EqualTo(1_000_000_000L));

        // Level should be capped at max (99 with default settings)
        Assert.That(dbPlayer.Skills.AttackLevel, Is.EqualTo(99));
    }

    /// <summary>
    /// Verifies that multiple players are isolated correctly.
    /// </summary>
    [Test]
    public async Task MultiplePlayersIsolated()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(MultiplePlayersIsolated));
        var playerService = factory.CreatePlayerService();

        // Create two different players
        var player1 = await playerService.GetOrCreateAsync(11111UL, "Player1");
        var player2 = await playerService.GetOrCreateAsync(22222UL, "Player2");

        // Give them different XP
        player1.AttackXpEarned = 100;
        player2.MagicXpEarned = 200;

        await playerService.SaveProgressAsync(player1, dungeonLevel: 1, died: false);
        await playerService.SaveProgressAsync(player2, dungeonLevel: 1, died: false);

        // Assert - verify isolation
        var db1 = await playerService.GetPlayerDataAsync(11111UL);
        var db2 = await playerService.GetPlayerDataAsync(22222UL);

        Assert.That(db1!.AttackXpTotal, Is.EqualTo(100));
        Assert.That(db1.MagicXpTotal, Is.EqualTo(0));

        Assert.That(db2!.AttackXpTotal, Is.EqualTo(0));
        Assert.That(db2.MagicXpTotal, Is.EqualTo(200));
    }

    #endregion
}
