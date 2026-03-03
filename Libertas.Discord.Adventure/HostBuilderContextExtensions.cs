using Libertas.Discord.Adventure.Data;
using Libertas.Discord.Adventure.Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Libertas.Discord.Adventure;

/// <summary>
///     Extension methods to configure and register adventure services into the host's
///     <see cref="IServiceCollection" />.
/// </summary>
public static class HostBuilderContextExtensions
{
    /// <summary>
    ///     Registers adventure services, EF Core context and related configuration sections
    ///     into the provided <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="hostContext">Host builder context containing configuration.</param>
    /// <param name="services">Service collection to configure.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAdventure(this HostBuilderContext hostContext, IServiceCollection services)
    {
        var configuration = hostContext.Configuration;

        services.AddHostedService<DatabaseInitializer>();

        services.AddDbContextFactory<AdventureContext>(options => options.UseSqlite("Data Source=adventure.db").UseLazyLoadingProxies());

        // Register Discord services via the Discord project's ServiceCollectionExtensions
        services.AddDiscordAdventure(
            configuration.GetSection("Discord"),
            configuration.GetSection("Localization"),
            configuration.GetSection("Bot"),
            configuration.GetSection("Progression"),
            configuration.GetSection("Combat"));

        return services;
    }
}