using Microsoft.EntityFrameworkCore;

namespace Libertas.Discord.Adventure.Data;

/// <summary>
///     EF Core <see cref="DbContext" /> for the adventure application.
///     Applies entity configurations from the data assembly.
/// </summary>
public class AdventureContext(DbContextOptions<AdventureContext> options) : DbContext(options)
{
    /// <summary>
    ///     Configure the model by applying all IEntityTypeConfiguration implementations
    ///     from the data assembly.
    /// </summary>
    /// <param name="modelBuilder">Model builder provided by EF Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}