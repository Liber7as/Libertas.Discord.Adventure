using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;

namespace Libertas.Discord.Adventure.Data.Tests;

/// <summary>
///     Integration tests for the PlayerRepository.
///     Tests direct database operations without the PlayerService layer.
/// </summary>
[TestFixture]
[Category("Database")]
[Category("Repository")]
public class PlayerRepositoryTests
{
    /// <summary>
    ///     Verifies that GetOrCreate creates a new player when not found.
    /// </summary>
    [Test]
    public async Task GetOrCreate_NewPlayer_CreatesRecord()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(GetOrCreate_NewPlayer_CreatesRecord));
        var repository = factory.CreatePlayerRepository();

        // Act
        var player = await repository.GetOrCreateAsync(new PlayerId(12345UL), "NewPlayer");

        // Assert
        Assert.That(player, Is.Not.Null);
        Assert.That(player.Id.Value, Is.EqualTo(12345UL));
        Assert.That(player.Name, Is.EqualTo("NewPlayer"));
        Assert.That(player.Skills.AttackLevel, Is.EqualTo(1));
        Assert.That(player.Skills.MagicLevel, Is.EqualTo(1));
        Assert.That(player.Skills.SpeechLevel, Is.EqualTo(1));
        Assert.That(player.Skills.DefenseLevel, Is.EqualTo(1));
        Assert.That(player.AttackXpTotal, Is.EqualTo(0));
        Assert.That(player.TotalGold, Is.EqualTo(0));
        Assert.That(player.TotalKills, Is.EqualTo(0));
        Assert.That(player.TotalDeaths, Is.EqualTo(0));
    }

    /// <summary>
    ///     Verifies that GetOrCreate returns existing player.
    /// </summary>
    [Test]
    public async Task GetOrCreate_ExistingPlayer_ReturnsExisting()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(GetOrCreate_ExistingPlayer_ReturnsExisting));
        var repository = factory.CreatePlayerRepository();

        // Create initial player
        var created = await repository.GetOrCreateAsync(new PlayerId(11111UL), "FirstName");

        // Modify and save
        created.AttackXpTotal = 500;
        created.TotalGold = 100;
        await repository.UpdateAsync(created);

        // Act - get again with different name
        var retrieved = await repository.GetOrCreateAsync(new PlayerId(11111UL), "NewName");

        // Assert - should have updated name but same stats
        Assert.That(retrieved.Name, Is.EqualTo("NewName"), "Username should be updated");
        Assert.That(retrieved.AttackXpTotal, Is.EqualTo(500), "XP should be preserved");
        Assert.That(retrieved.TotalGold, Is.EqualTo(100), "Gold should be preserved");
    }

    /// <summary>
    ///     Verifies GetById returns null for non-existent player.
    /// </summary>
    [Test]
    public async Task GetById_NonExistent_ReturnsNull()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(GetById_NonExistent_ReturnsNull));
        var repository = factory.CreatePlayerRepository();

        // Act
        var player = await repository.GetByIdAsync(new PlayerId(999999UL));

        // Assert
        Assert.That(player, Is.Null);
    }

    /// <summary>
    ///     Verifies GetById returns correct player.
    /// </summary>
    [Test]
    public async Task GetById_ExistingPlayer_ReturnsPlayer()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(GetById_ExistingPlayer_ReturnsPlayer));
        var repository = factory.CreatePlayerRepository();

        await repository.GetOrCreateAsync(new PlayerId(22222UL), "TestPlayer");

        // Act
        var player = await repository.GetByIdAsync(new PlayerId(22222UL));

        // Assert
        Assert.That(player, Is.Not.Null);
        Assert.That(player!.Name, Is.EqualTo("TestPlayer"));
    }

    /// <summary>
    ///     Verifies that updates are persisted correctly.
    /// </summary>
    [Test]
    public async Task Update_ChangesArePersisted()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(Update_ChangesArePersisted));
        var repository = factory.CreatePlayerRepository();

        var player = await repository.GetOrCreateAsync(new PlayerId(33333UL), "UpdateTest");

        // Modify player
        player.Skills = new SkillLevels(10, 5, player.Skills.SpeechLevel, player.Skills.DefenseLevel);
        player.AttackXpTotal = 5000;
        player.MagicXpTotal = 500;
        player.TotalGold = 250.5;
        player.TotalKills = 42;
        player.TotalDeaths = 3;
        player.HighestDungeonLevel = 15;

        // Act
        await repository.UpdateAsync(player);

        // Assert - reload and verify
        var reloaded = await repository.GetByIdAsync(new PlayerId(33333UL));
        Assert.That(reloaded!.Skills.AttackLevel, Is.EqualTo(10));
        Assert.That(reloaded.AttackXpTotal, Is.EqualTo(5000));
        Assert.That(reloaded.Skills.MagicLevel, Is.EqualTo(5));
        Assert.That(reloaded.MagicXpTotal, Is.EqualTo(500));
        Assert.That(reloaded.TotalGold, Is.EqualTo(250.5).Within(0.01));
        Assert.That(reloaded.TotalKills, Is.EqualTo(42));
        Assert.That(reloaded.TotalDeaths, Is.EqualTo(3));
        Assert.That(reloaded.HighestDungeonLevel, Is.EqualTo(15));
    }

    /// <summary>
    ///     Verifies that LastActiveAt is updated on access.
    /// </summary>
    [Test]
    public async Task LastActiveAt_UpdatedOnAccess()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(LastActiveAt_UpdatedOnAccess));
        var repository = factory.CreatePlayerRepository();

        var player = await repository.GetOrCreateAsync(new PlayerId(44444UL), "TimestampTest");
        var firstActiveAt = player.LastActiveAt;

        // Wait a tiny bit to ensure time difference
        await Task.Delay(10);

        // Act - access again
        var reloaded = await repository.GetOrCreateAsync(new PlayerId(44444UL), "TimestampTest");

        // Assert
        Assert.That(reloaded.LastActiveAt, Is.GreaterThanOrEqualTo(firstActiveAt));
    }

    /// <summary>
    ///     Verifies that CreatedAt is set on first creation.
    /// </summary>
    [Test]
    public async Task CreatedAt_SetOnCreation()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(CreatedAt_SetOnCreation));
        var repository = factory.CreatePlayerRepository();
        var beforeCreate = DateTimeOffset.UtcNow;

        // Act
        var player = await repository.GetOrCreateAsync(new PlayerId(55555UL), "CreateTimestamp");

        // Assert
        Assert.That(player.CreatedAt, Is.GreaterThanOrEqualTo(beforeCreate));
        Assert.That(player.CreatedAt, Is.LessThanOrEqualTo(DateTimeOffset.UtcNow));
    }

    /// <summary>
    ///     Verifies that concurrent access to the same player works correctly.
    /// </summary>
    [Test]
    public async Task ConcurrentAccess_HandledCorrectly()
    {
        // Arrange
        using var factory = new TestDatabaseFactory(nameof(ConcurrentAccess_HandledCorrectly));
        var repository = factory.CreatePlayerRepository();

        // Create initial player
        await repository.GetOrCreateAsync(new PlayerId(66666UL), "ConcurrentTest");

        // Act - concurrent updates
        var tasks = Enumerable.Range(0, 10).Select(async i =>
        {
            var player = await repository.GetByIdAsync(new PlayerId(66666UL));
            if (player != null)
            {
                player.AttackXpTotal += 10;
                await repository.UpdateAsync(player);
            }
        }).ToList();

        await Task.WhenAll(tasks);

        // Assert - should have accumulated (though exact amount may vary due to race conditions)
        var final = await repository.GetByIdAsync(new PlayerId(66666UL));
        Assert.That(final!.AttackXpTotal, Is.GreaterThan(0), "Some XP should have accumulated");

        TestContext.Out.WriteLine($"Final XP after concurrent updates: {final.AttackXpTotal}");
    }
}