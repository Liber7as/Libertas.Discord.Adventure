using Libertas.Discord.Adventure.Core.Data;
using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libertas.Discord.Adventure.Data.Adapters;

/// <summary>
///     Adapter that implements core-facing IPlayerRepository using the EF Player entity.
///     Maps between persistent Player entity and core PlayerState.
/// </summary>
/// <summary>
///     Repository adapter that maps EF <see cref="Player" /> entities to core <see cref="PlayerState" /> objects
///     and persists changes from core back into the database.
/// </summary>
public class PlayerRepositoryAdapter(IDbContextFactory<AdventureContext> dbContextFactory, ILogger<PlayerRepositoryAdapter> logger) : IPlayerRepository
{
    private readonly IDbContextFactory<AdventureContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    private readonly ILogger<PlayerRepositoryAdapter> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Loads a player by id and maps it to a <see cref="PlayerState" />. Returns <c>null</c> if not found.
    /// </summary>
    public async Task<PlayerState?> GetByIdAsync(PlayerId id, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var player = await ctx.Set<Player>().FindAsync([id.Value], cancellationToken);
        if (player == null)
        {
            return null;
        }

        var state = MapToState(player);
        PopulatePersistentFields(state, player);
        return state;
    }

    // removed convenience overloads accepting raw ulong ids

    // Legacy entity-returning APIs removed; core code should use PlayerState via IPlayerRepository

    /// <summary>
    ///     Retrieves an existing player state or creates a new persistent player record when none exists.
    /// </summary>
    public async Task<PlayerState> GetOrCreateAsync(PlayerId id, string username, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var player = await ctx.Set<Player>().FindAsync([id.Value], cancellationToken);

        if (player != null)
        {
            player.Username = username;
            player.LastActiveAt = DateTimeOffset.UtcNow;
            await ctx.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Loaded existing player {PlayerId} ({Username}) from DB", id.Value, username);
            var existingState = MapToState(player);
            PopulatePersistentFields(existingState, player);
            return existingState;
        }

        player = new Player
        {
            Id = id.Value,
            Username = username,
            AttackLevel = 1,
            AttackXp = 0,
            MagicLevel = 1,
            MagicXp = 0,
            SpeechLevel = 1,
            SpeechXp = 0,
            DefenseLevel = 1,
            DefenseXp = 0,
            TotalGold = 0,
            TotalKills = 0,
            TotalDeaths = 0,
            HighestDungeonLevel = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            LastActiveAt = DateTimeOffset.UtcNow
        };

        ctx.Set<Player>().Add(player);
        await ctx.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created new player {PlayerId} ({Username})", id.Value, username);
        var newState = MapToState(player);
        PopulatePersistentFields(newState, player);
        return newState;
    }

    // removed convenience overloads accepting raw ulong ids

    // Legacy entity-returning APIs removed; core code should use PlayerState via IPlayerRepository

    /// <summary>
    ///     Persists changes from a <see cref="PlayerState" /> back to the underlying <see cref="Player" /> entity.
    /// </summary>
    public async Task UpdateAsync(PlayerState playerState, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var player = await ctx.Set<Player>().FindAsync([playerState.Id.Value], cancellationToken);
        if (player == null)
        {
            _logger.LogWarning("Player {PlayerId} not found when updating. Creating new entity.", playerState.Id.Value);
            player = new Player { Id = playerState.Id.Value, Username = playerState.Name };
            ctx.Set<Player>().Add(player);
        }

        // Persist XP and totals
        // Prefer explicit total values when present (used by direct repository callers/tests),
        // otherwise apply session-earned deltas.
        // If caller changed the total value compared to the stored entity, treat as explicit set.
        if (playerState.AttackXpTotal != player.AttackXp)
        {
            player.AttackXp = playerState.AttackXpTotal;
        }
        else if (playerState.AttackXpEarned != 0)
        {
            player.AttackXp += playerState.AttackXpEarned;
        }

        if (playerState.MagicXpTotal != player.MagicXp)
        {
            player.MagicXp = playerState.MagicXpTotal;
        }
        else if (playerState.MagicXpEarned != 0)
        {
            player.MagicXp += playerState.MagicXpEarned;
        }

        if (playerState.SpeechXpTotal != player.SpeechXp)
        {
            player.SpeechXp = playerState.SpeechXpTotal;
        }
        else if (playerState.SpeechXpEarned != 0)
        {
            player.SpeechXp += playerState.SpeechXpEarned;
        }

        if (playerState.DefenseXpTotal != player.DefenseXp)
        {
            player.DefenseXp = playerState.DefenseXpTotal;
        }
        else if (playerState.DefenseXpEarned != 0)
        {
            player.DefenseXp += playerState.DefenseXpEarned;
        }

        // Update levels if progression logic elsewhere updated them in state
        player.AttackLevel = playerState.Skills.AttackLevel;
        player.MagicLevel = playerState.Skills.MagicLevel;
        player.SpeechLevel = playerState.Skills.SpeechLevel;
        player.DefenseLevel = playerState.Skills.DefenseLevel;

        // Gold and kills: prefer explicit totals if provided, otherwise apply earned deltas
        if (playerState.TotalGold != player.TotalGold)
        {
            player.TotalGold = playerState.TotalGold;
        }
        else if (playerState.GoldEarned != 0)
        {
            player.TotalGold += playerState.GoldEarned;
        }

        if (playerState.TotalKills != player.TotalKills)
        {
            player.TotalKills = playerState.TotalKills;
        }
        else if (playerState.MobsKilled != 0)
        {
            player.TotalKills += playerState.MobsKilled;
        }

        // Highest dungeon and deaths: prefer explicit totals when provided
        if (playerState.HighestDungeonLevel != player.HighestDungeonLevel)
        {
            player.HighestDungeonLevel = playerState.HighestDungeonLevel;
        }

        if (playerState.TotalDeaths != player.TotalDeaths)
        {
            player.TotalDeaths = playerState.TotalDeaths;
        }

        // Update last active timestamp
        player.LastActiveAt = DateTimeOffset.UtcNow;

        await ctx.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Updated player {PlayerId} in DB", playerState.Id.Value);
    }

    // Legacy entity-returning APIs removed; core code should use PlayerState via IPlayerRepository

    /// <summary>
    ///     Returns the top players ordered by the sum of their skill levels.
    /// </summary>
    public async Task<List<PlayerState>> GetTopPlayersByTotalLevelAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var players = await ctx.Set<Player>()
            .AsNoTracking()
            .OrderByDescending(p => p.AttackLevel + p.MagicLevel + p.SpeechLevel + p.DefenseLevel)
            .Take(count)
            .ToListAsync(cancellationToken);

        return [.. players.Select(MapToState)];
    }

    /// <summary>
    ///     Returns the top players ordered by highest dungeon reached.
    /// </summary>
    public async Task<List<PlayerState>> GetTopPlayersByHighestDungeonAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var players = await ctx.Set<Player>()
            .AsNoTracking()
            .OrderByDescending(p => p.HighestDungeonLevel)
            .Take(count)
            .ToListAsync(cancellationToken);

        return [.. players.Select(MapToState)];
    }

    /// <summary>
    ///     Returns the top players ordered by total gold.
    /// </summary>
    public async Task<List<PlayerState>> GetTopPlayersByGoldAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var players = await ctx.Set<Player>()
            .AsNoTracking()
            .OrderByDescending(p => p.TotalGold)
            .Take(count)
            .ToListAsync(cancellationToken);

        return [.. players.Select(MapToState)];
    }

    // Legacy entity-returning APIs removed; core code should use PlayerState via IPlayerRepository

    // Legacy entity-returning APIs removed; core code should use PlayerState via IPlayerRepository

    private static PlayerState MapToState(Player player)
    {
        var skills = new SkillLevels(player.AttackLevel, player.MagicLevel, player.SpeechLevel, player.DefenseLevel);

        return new PlayerState
        {
            Id = new PlayerId(player.Id),
            Name = player.Username,
            Skills = skills,
            // Combat stats will be calculated by core progression service when starting a session
            MaxHp = 20,
            CurrentHp = 20,
            AttackPower = new PowerLevel(1),
            MagicPower = new PowerLevel(1),
            SpeechPower = new PowerLevel(1),
            DefensePower = new PowerLevel(1)
        };
    }

    private static void PopulatePersistentFields(PlayerState state, Player player)
    {
        state.AttackXpTotal = player.AttackXp;
        state.MagicXpTotal = player.MagicXp;
        state.SpeechXpTotal = player.SpeechXp;
        state.DefenseXpTotal = player.DefenseXp;

        state.TotalGold = player.TotalGold;
        state.TotalKills = player.TotalKills;
        state.HighestDungeonLevel = player.HighestDungeonLevel;
        state.TotalDeaths = player.TotalDeaths;
        state.CreatedAt = player.CreatedAt;
        state.LastActiveAt = player.LastActiveAt;
    }
}