using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Libertas.Discord.Adventure.Discord;

/// <summary>
/// Background service that initializes and runs the Discord client and command modules.
/// </summary>
public sealed class DiscordBackgroundService(IOptions<DiscordSettings> options, ILogger<DiscordBackgroundService> logger, DiscordEventHandler eventHandler, DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ILogger<DiscordBackgroundService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly DiscordSettings _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly DiscordEventHandler _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
    private readonly DiscordSocketClient _client = client ?? throw new ArgumentNullException(nameof(client));
    private readonly CommandService _commandService = commandService;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    /// <summary>
    /// Starts the Discord client, registers command modules and wires up event handlers.
    /// The method blocks until cancellation is requested.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Add all modules used by this assembly
        await _commandService.AddModulesAsync(GetType().Assembly, _serviceProvider);

        // Wire up basic events
        _client.MessageReceived += message => _eventHandler.OnMessageReceivedAsync(message, stoppingToken);
        _client.ButtonExecuted += component => _eventHandler.OnButtonExecutedAsync(component, stoppingToken);

        var token = _settings.Token;
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogError("Discord token configuration is missing.");
            throw new InvalidOperationException("Discord token is not configured.");
        }

        try
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _logger.LogInformation("Discord client started.");

            // Keep the service running until cancellation is requested.
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // expected during shutdown, swallow
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while starting the Discord client. If this was a login failure, verify the bot token and intents.");
            throw;
        }
    }

    /// <summary>
    /// Stops and logs out the Discord client if connected, then delegates to base.StopAsync.
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_client.ConnectionState == ConnectionState.Connected)
        {
            try
            {
                await _client.LogoutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occurred while logging out the Discord client.");
            }

            try
            {
                await _client.StopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occurred while stopping the Discord client.");
            }
        }

        await base.StopAsync(cancellationToken);
    }
}