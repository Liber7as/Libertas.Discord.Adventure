using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Actions;
using Microsoft.Extensions.Logging;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
///     Resolves combat actions by delegating to specialized action handlers.
///     Uses the Strategy pattern - each action type has its own handler implementation.
/// </summary>
/// <remarks>
///     <para>
///         This service acts as a dispatcher, routing player and mob actions to the appropriate handlers.
///         All game logic is contained within the individual action handlers.
///     </para>
///     <para>
///         At construction time, validates that all <see cref="PlayerAction" /> enum values have
///         corresponding handlers to catch configuration errors early.
///     </para>
/// </remarks>
public class ActionResolutionService : IActionResolutionService
{
    private readonly ILogger<ActionResolutionService> _logger;
    private readonly IMobActionHandler _mobHandler;
    private readonly Dictionary<PlayerAction, IPlayerActionHandler> _playerHandlers;

    /// <summary>
    ///     Creates a new ActionResolutionService with the provided handlers.
    /// </summary>
    /// <param name="playerHandlers">Collection of player action handlers (one per action type).</param>
    /// <param name="mobHandler">Handler for mob attack actions.</param>
    /// <param name="logger">Logger for action dispatch events.</param>
    /// <exception cref="InvalidOperationException">Thrown if any PlayerAction enum value lacks a handler.</exception>
    public ActionResolutionService(
        IEnumerable<IPlayerActionHandler> playerHandlers,
        IMobActionHandler mobHandler,
        ILogger<ActionResolutionService> logger)
    {
        ArgumentNullException.ThrowIfNull(playerHandlers);
        ArgumentNullException.ThrowIfNull(mobHandler);
        ArgumentNullException.ThrowIfNull(logger);

        _playerHandlers = playerHandlers.ToDictionary(h => h.Action);
        _mobHandler = mobHandler;
        _logger = logger;

        // Validate all actions have handlers at startup
        var missingActions = Enum.GetValues<PlayerAction>()
            .Where(a => !_playerHandlers.ContainsKey(a))
            .ToList();

        if (missingActions.Count > 0)
        {
            var errorMessage = $"Missing handlers for actions: {string.Join(", ", missingActions)}";
            _logger.LogCritical("Action handler validation failed: {Error}", errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _logger.LogDebug(
            "ActionResolutionService initialized with {HandlerCount} player action handlers",
            _playerHandlers.Count);
    }

    /// <inheritdoc />
    public void HandlePlayerAction(CombatContext<PlayerState> context, PlayerAction action)
    {
        if (_playerHandlers.TryGetValue(action, out var handler))
        {
            _logger.LogTrace(
                "Executing {Action} for player {PlayerName} ({PlayerId})",
                action, context.Actor.Name, context.Actor.Id.Value);

            handler.Execute(context);
        }
        else
        {
            // This shouldn't happen due to constructor validation, but log if it does
            _logger.LogError(
                "No handler found for action {Action} requested by player {PlayerName}",
                action, context.Actor.Name);
        }
    }

    /// <inheritdoc />
    public void HandleMobAction(CombatContext<MobState> context)
    {
        _logger.LogTrace("Executing attack for mob {MobName} ({MobId})", context.Actor.Name, context.Actor.Id.Value);
        _mobHandler.Execute(context);
    }
}