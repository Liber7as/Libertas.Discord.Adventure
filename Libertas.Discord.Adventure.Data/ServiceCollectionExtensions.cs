using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Libertas.Discord.Adventure.Data;

/// <summary>
///     Extension methods for registering data-layer services into an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers data-related services for the adventure application using a SQLite provider.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="connectionString">SQLite connection string.</param>
    public static void AddAdventureDataSqlite(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<AdventureContext>(options => options.UseSqlite(connectionString, b => b.MigrationsAssembly(typeof(ServiceCollectionExtensions).Assembly)).UseLazyLoadingProxies());
    }
}