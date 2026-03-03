using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Core.Settings;
using Libertas.Discord.Adventure.Data; // Updated to ensure file is modified
using Libertas.Discord.Adventure.Core.Data;
using Libertas.Discord.Adventure.Discord.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Libertas.Discord.Adventure.Data.Tests;

/// <summary>
/// Factory for creating test instances with in-memory database.
/// Each test should create its own factory instance to ensure isolation.
/// </summary>
public class TestDatabaseFactory : IDisposable
{
    private readonly DbContextOptions<AdventureContext> _options;
    private readonly string _databaseName;

    /// <summary>
    /// Creates a new test database factory with a unique in-memory database.
    /// </summary>
    /// <param name="testName">Name of the test (used for database naming).</param>
    public TestDatabaseFactory(string testName)
    {
        _databaseName = $"TestDb_{testName}_{Guid.NewGuid()}";
        _options = new DbContextOptionsBuilder<AdventureContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;
    }

    /// <summary>
    /// Creates a new database context for the test database.
    /// </summary>
    public AdventureContext CreateContext()
    {
        return new AdventureContext(_options);
    }

    /// <summary>
    /// Creates a DbContextFactory for use with services that require one.
    /// </summary>
    public IDbContextFactory<AdventureContext> CreateDbContextFactory()
    {
        return new TestDbContextFactory(_options);
    }

    /// <summary>
    /// Creates a fully configured PlayerRepository for testing.
    /// </summary>
    public Libertas.Discord.Adventure.Core.Data.IPlayerRepository CreatePlayerRepository()
    {
        return new Libertas.Discord.Adventure.Data.Adapters.PlayerRepositoryAdapter(
            CreateDbContextFactory(),
            NullLogger<Libertas.Discord.Adventure.Data.Adapters.PlayerRepositoryAdapter>.Instance);
    }

    /// <summary>
    /// Creates a fully configured PlayerService for testing.
    /// </summary>
    public IPlayerService CreatePlayerService(ProgressionSettings? progressionSettings = null)
    {
        progressionSettings ??= CreateDefaultProgressionSettings();
        var progressionService = new PlayerProgressionService(Options.Create(progressionSettings));

        var repo = CreatePlayerRepository();
        return new PlayerService(
            (Libertas.Discord.Adventure.Core.Data.IPlayerRepository)repo,
            progressionService,
            NullLogger<PlayerService>.Instance);
    }

    /// <summary>
    /// Creates a PlayerProgressionService for testing.
    /// </summary>
    public IPlayerProgressionService CreateProgressionService(ProgressionSettings? settings = null)
    {
        settings ??= CreateDefaultProgressionSettings();
        return new PlayerProgressionService(Options.Create(settings));
    }

    /// <summary>
    /// Default progression settings for tests.
    /// </summary>
    public static ProgressionSettings CreateDefaultProgressionSettings() => new()
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

    public void Dispose()
    {
        // In-memory databases are automatically cleaned up when all contexts are disposed
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Simple DbContextFactory implementation for testing.
    /// </summary>
    private class TestDbContextFactory(DbContextOptions<AdventureContext> options) : IDbContextFactory<AdventureContext>
    {
        private readonly DbContextOptions<AdventureContext> _options = options;

        public AdventureContext CreateDbContext()
        {
            return new AdventureContext(_options);
        }
    }
}
