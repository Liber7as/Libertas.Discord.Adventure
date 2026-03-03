using Libertas.Discord.Adventure.Core.Data;
using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Data;
using Libertas.Discord.Adventure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Libertas.Discord.Adventure.Core.Services;

namespace Libertas.Discord.Adventure.Data.Adapters;

/// <summary>
/// Adapter that implements <see cref="Libertas.Discord.Adventure.Core.Data.IMobPresetService"/>
/// using the EF Core <see cref="AdventureContext"/>. Returns lightweight DTOs used by core.
/// </summary>
public class MobPresetServiceAdapter(IDbContextFactory<AdventureContext> dbContextFactory, IRandomNumberGenerator rng, ILogger<MobPresetServiceAdapter> logger) : IMobPresetService
{
    private readonly IDbContextFactory<AdventureContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    private readonly IRandomNumberGenerator _rng = rng ?? throw new ArgumentNullException(nameof(rng));
    private readonly ILogger<MobPresetServiceAdapter> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Gets a mob preset entity by id, or <c>null</c> when not found.
    /// </summary>
    public async Task<Libertas.Discord.Adventure.Data.Entities.MobPreset?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await ctx.Set<Libertas.Discord.Adventure.Data.Entities.MobPreset>().FindAsync([id], cancellationToken);
    }

    /// <summary>
    /// Returns all mob preset entities.
    /// </summary>
    public async Task<IReadOnlyList<Libertas.Discord.Adventure.Data.Entities.MobPreset>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await ctx.Set<Libertas.Discord.Adventure.Data.Entities.MobPreset>().AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Selects and returns a random <see cref="MobPresetDto"/>, optionally excluding given preset ids.
    /// </summary>
    public async Task<MobPresetDto?> GetRandomAsync(IReadOnlyCollection<int>? excludeIds = null, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = ctx.Set<MobPreset>().AsNoTracking();
        if (excludeIds != null)
        {
            query = query.Where(m => !excludeIds.Contains(m.Id));
        }

        var total = await query.CountAsync(cancellationToken);
        if (total == 0)
        {
            return null;
        }

        var skip = _rng.Next(0, total);
        var selected = await query.OrderBy(m => m.Id).Skip(skip).Take(1).SingleOrDefaultAsync(cancellationToken);
        if (selected == null)
        {
            return null;
        }

        return new MobPresetDto(selected.Id, selected.Name, selected.MinHealth, selected.MaxHealth, selected.MinAttackPower, selected.MaxAttackPower, selected.ImageUrl);
    }
}
