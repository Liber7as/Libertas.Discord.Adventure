using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Core.Services.Actions;
using Libertas.Discord.Adventure.Core.Services.Combat;
using Libertas.Discord.Adventure.Core.Settings;
using Libertas.Discord.Adventure.Discord.Data;
using Libertas.Discord.Adventure.Discord.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Libertas.Discord.Adventure.Discord;

/// <summary>
///     Extension methods for registering Discord Adventure services and configuration with DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers all Discord Adventure services, settings, and dependencies with the DI container.
    /// </summary>
    /// <param name="services">Service collection to register with.</param>
    /// <param name="discordSection">Discord configuration section.</param>
    /// <param name="localizationSection">Localization configuration section.</param>
    /// <param name="botSection">Bot configuration section.</param>
    /// <param name="progressionSection">Progression configuration section.</param>
    /// <param name="combatSection">Combat configuration section.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDiscordAdventure(
        this IServiceCollection services,
        IConfigurationSection discordSection,
        IConfigurationSection localizationSection,
        IConfigurationSection botSection,
        IConfigurationSection progressionSection,
        IConfigurationSection combatSection)
    {
        // Bind settings using the options pattern for configuration
        services.AddOptions<DiscordSettings>()
            .Bind(discordSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<LocalizationSettings>()
            .Bind(localizationSection);

        services.AddOptions<BotSettings>()
            .Bind(botSection);

        services.AddOptions<ProgressionSettings>()
            .Bind(progressionSection);

        services.AddOptions<CombatSettings>()
            .Bind(combatSection);

        // Discord client configuration
        services.AddSingleton(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        });

        // Discord client and command service
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<CommandService>();

        // Background work queue for offloading CPU/IO work
        services.AddSingleton<IBackgroundWorkQueue, BackgroundWorkQueue>();
        services.AddHostedService<QueuedBackgroundService>();

        // Discord hosted services for event handling and bot lifecycle
        services.AddSingleton<DiscordEventHandler>();
        services.AddHostedService<DiscordBackgroundService>();

        // Core game logic services
        services.AddSingleton<IRandomNumberGenerator, RandomNumberGenerator>();
        services.AddSingleton<IActionLocalizationService, ActionLocalizationService>();
        services.AddSingleton<IPlayerProgressionService, PlayerProgressionService>();
        services.AddSingleton<IBotService, BotService>();

        // Combat calculator (uses CombatSettings)
        services.AddSingleton<ICombatCalculator, CombatCalculator>();

        // Combat helpers (distributors and appliers)
        services.AddSingleton<IXpDistributor, XpDistributor>();
        services.AddSingleton<ILootDistributor, LootDistributor>();
        services.AddSingleton<IDamageApplier, DamageApplier>();

        // Player action handlers (Strategy pattern)
        services.AddSingleton<IPlayerActionHandler, AttackActionHandler>();
        services.AddSingleton<IPlayerActionHandler, MagicActionHandler>();
        services.AddSingleton<IPlayerActionHandler, TalkActionHandler>();
        services.AddSingleton<IPlayerActionHandler, PrayActionHandler>();
        services.AddSingleton<IPlayerActionHandler, HealActionHandler>();
        services.AddSingleton<IPlayerActionHandler, RunActionHandler>();

        // Mob action handler
        services.AddSingleton<IMobActionHandler, MobActionHandler>();

        // Action resolution service (delegates to handlers)
        services.AddSingleton<IActionResolutionService, ActionResolutionService>();

        // Game engine and session/message management
        services.AddSingleton<IGameEngine, GameEngine>();
        services.AddSingleton<MessageService>();
        services.AddSingleton<AdventureSessionManager>();

        // Data services (scoped for EF Core)
        services.AddScoped<IMobFactoryService, MobFactoryService>();
        services.AddScoped<IPlayerService, PlayerService>();

        // Core-facing data adapters (implementations live in the Data project)
        services.AddScoped<Libertas.Discord.Adventure.Core.Data.IPlayerRepository, Libertas.Discord.Adventure.Data.Adapters.PlayerRepositoryAdapter>();
        services.AddScoped<Libertas.Discord.Adventure.Core.Data.IMobPresetService, Libertas.Discord.Adventure.Data.Adapters.MobPresetServiceAdapter>();

        return services;
    }
}