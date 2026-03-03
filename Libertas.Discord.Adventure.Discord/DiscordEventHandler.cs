using Discord.Commands;
using Discord.WebSocket;
using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Discord.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Libertas.Discord.Adventure.Discord;

/// <summary>
/// Handles Discord events (messages and button interactions) for the adventure bot.
/// Routes text commands to the command service and combat actions to the session manager.
/// </summary>
/// <remarks>
/// <para>Message handling and button interactions are queued to a background worker
/// to avoid blocking the Discord gateway connection.</para>
/// <para>Button interactions are parsed from custom IDs in the format "action-{actionId}-{channelId}".</para>
/// </remarks>
/// <remarks>
/// Creates a new DiscordEventHandler instance.
/// </remarks>
public sealed class DiscordEventHandler(
    DiscordSocketClient client,
    CommandService commandService,
    IBackgroundWorkQueue workQueue,
    MessageService messageService,
    AdventureSessionManager sessionManager,
    IServiceProvider serviceProvider,
    IOptions<DiscordSettings> options,
    ILogger<DiscordEventHandler> logger)
{
    private readonly DiscordSocketClient _client = client ?? throw new ArgumentNullException(nameof(client));
    private readonly CommandService _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
    private readonly ILogger<DiscordEventHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly AdventureSessionManager _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
    private readonly DiscordSettings _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly IBackgroundWorkQueue _workQueue = workQueue ?? throw new ArgumentNullException(nameof(workQueue));
    private readonly MessageService _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));

    /// <summary>
    /// Handles incoming Discord messages, routing commands to the command service.
    /// Non-command messages are ignored.
    /// </summary>
    /// <param name="message">The received message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task OnMessageReceivedAsync(SocketMessage message, CancellationToken cancellationToken)
    {
        try
        {
            _ = _workQueue.QueueAsync(async ct =>
            {
                var author = message.Author;

                if (author == null)
                {
                    return;
                }

                if (author.IsBot)
                {
                    return;
                }

                // Only process user messages
                if (message is not SocketUserMessage userMessage)
                {
                    return;
                }

                // Create a number to track where the prefix ends and the command begins
                var argPos = 0;

                // Determine if the message is a command based on the prefix or mention (match AdventureService behavior)
                if (!(userMessage.HasStringPrefix(_settings.Prefix, ref argPos) || userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                {
                    return; // not a command
                }

                var cmdContext = new SocketCommandContext(_client, userMessage);

                var result = await _commandService.ExecuteAsync(cmdContext, argPos, _serviceProvider);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    // Include enum value for easier debugging
                    _logger.LogWarning("Command execution failed: {Error} - {Reason}", result.Error, result.ErrorReason);

                    // Send the error reason back to the channel
                    await cmdContext.Channel.SendMessageAsync(result.ErrorReason);
                }
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while handling MessageReceived event");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles combat action button clicks from players.
    /// Parses the action from the button's custom ID and records it in the session.
    /// </summary>
    /// <param name="component">The button interaction component.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task OnButtonExecutedAsync(SocketMessageComponent component, CancellationToken cancellationToken)
    {
        try
        {
            // Acknowledge the interaction immediately to prevent Discord timeout (3 second limit)
            await component.DeferAsync();

            // Offload processing of the interaction to the background queue
            _ = _workQueue.QueueAsync(async ct =>
            {
                var user = component.User;

                if (user.IsBot)
                {
                    return;
                }

                if (component.ChannelId == null)
                {
                    return;
                }

                // Try to parse the custom id from the button (e.g. "action-1-123456789012345678")
                var customId = component.Data?.CustomId ?? string.Empty;

                if (!component.TryParseActionId(out var action, out var channelId))
                {
                    _logger.LogWarning("Unrecognized button custom id: {CustomId}", customId);

                    try
                    {
                        await component.FollowupAsync("Unknown action.", ephemeral: true);
                    }
                    catch
                    {
                        // ignore followup failures
                    }

                    return;
                }

                // Groundwork only: log parsed values and confirm to the user.
                _logger.LogInformation("Parsed action button: Action={Action}, ChannelId={ChannelId}, User={UserId}", action, channelId, user.Id);

                if (!_sessionManager.Sessions.TryGetValue(component.ChannelId.Value, out var session))
                {
                    await component.FollowupAsync("Could not find active session.", ephemeral: true);

                    return;
                }

                // Check if the player is dead before recording their action
                var playerId = new PlayerId(user.Id);
                var existingPlayer = session.Players.FirstOrDefault(p => p.Id == playerId);
                if (existingPlayer != null && !existingPlayer.IsAlive)
                {
                    try
                    {
                        await component.FollowupAsync("💀 You have fallen in battle and cannot act. Wait for the next adventure!", ephemeral: true);
                    }
                    catch
                    {
                        // ignore followup failures
                    }
                    return;
                }

                await _sessionManager.RecordActionAsync(component.ChannelId.Value, user, action, ct);

                await _messageService.UpdateCombatMessageAsync(session);

                try
                {
                    await component.FollowupAsync($"You have chosen to **{action}**.", ephemeral: true);
                }
                catch
                {
                    // ignore followup failures
                }

            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing a button interaction.");

            try
            {
                // Attempt to respond to the interaction with an error message if possible
                await component.FollowupAsync("An error occurred processing your action.");
            }
            catch
            {
                // ignore secondary failures
            }
        }
    }
}