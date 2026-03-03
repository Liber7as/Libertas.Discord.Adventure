using System.Collections.Concurrent;
using Discord.WebSocket;
using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Discord.Data;
using Microsoft.Extensions.Logging;

namespace Libertas.Discord.Adventure.Discord.Services;

/// <summary>
///     Manages per-channel adventure sessions.
///     Provides thread-safe creation, lookup, action recording, round execution, and session termination.
/// </summary>
/// <remarks>
///     Sessions are stored in the <see cref="Sessions" /> dictionary keyed by Discord channel ID.
///     Public methods use internal locking to ensure session state is updated safely from concurrent Discord events.
/// </remarks>
public sealed class AdventureSessionManager(IGameEngine gameEngine, IPlayerService playerService, ILogger<AdventureSessionManager> logger)
{
    private readonly IGameEngine _gameEngine = gameEngine ?? throw new ArgumentNullException(nameof(gameEngine));
    private readonly ILogger<AdventureSessionManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IPlayerService _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));

    /// <summary>
    ///     Active sessions keyed by Discord channel ID.
    /// </summary>
    public ConcurrentDictionary<ulong, AdventureSession> Sessions { get; } = new();

    /// <summary>
    ///     Starts a new session for the specified channel and level.
    /// </summary>
    /// <param name="channelId">Discord channel id where the session will run.</param>
    /// <param name="level">Starting dungeon level for the session.</param>
    /// <param name="cancellationToken">Cancellation token to observe when creating the session.</param>
    /// <returns>The created <see cref="AdventureSession" />.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a session cannot be added for the channel.</exception>
    public Task<AdventureSession> StartSessionAsync(ulong channelId, int level, CancellationToken cancellationToken = default)
    {
        var session = new AdventureSession
        {
            ChannelId = channelId,
            Level = level,

            RoundCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
        };

        if (!Sessions.TryAdd(channelId, session))
        {
            throw new InvalidOperationException("Could not add game session.");
        }

        _logger.LogInformation("Started session in channel {ChannelId}.", channelId);

        return Task.FromResult(session);
    }

    /// <summary>
    ///     Records a player's action for the specified channel using a Discord <see cref="SocketUser" />.
    ///     Ensures the player entity exists and is added to the session before recording the action.
    /// </summary>
    /// <param name="channelId">Channel id where the action originated.</param>
    /// <param name="user">Discord user who performed the action.</param>
    /// <param name="action">Player action to record.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task RecordActionAsync(ulong channelId, SocketUser user, PlayerAction action, CancellationToken cancellationToken = default)
    {
        var playerId = new PlayerId(user.Id);

        // Ensure player exists
        await this.LockAsync(channelId, async (session, ct) =>
        {
            if (session.Players.All(p => p.Id != playerId))
            {
                var player = await _playerService.GetOrCreateAsync(user, ct);

                session.Players.Add(player);
            }
        }, cancellationToken);

        // Update actions
        await RecordActionAsync(channelId, playerId, action, cancellationToken);
    }

    /// <summary>
    ///     Records a player's action for the specified channel using a <see cref="PlayerId" />.
    ///     Updates the session's pending actions and last activity timestamp.
    /// </summary>
    /// <param name="channelId">Channel id where the action originated.</param>
    /// <param name="playerId">Player identifier.</param>
    /// <param name="action">Player action to record.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task RecordActionAsync(ulong channelId, PlayerId playerId, PlayerAction action, CancellationToken cancellationToken = default)
    {
        await this.LockAsync(channelId, async (session, ct) =>
        {
            session.PendingActions[playerId] = action;
            session.LastActivity = DateTimeOffset.UtcNow;
        }, cancellationToken);
    }

    /// <summary>
    ///     Executes a single round for the provided session by delegating to the <see cref="IGameEngine" />.
    ///     Updates the session state with the returned <see cref="RoundResult" />.
    /// </summary>
    /// <param name="session">Active session to execute a round for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The <see cref="RoundResult" /> describing the round outcome.</returns>
    public async Task<RoundResult> ExecuteRoundAsync(AdventureSession session, CancellationToken cancellationToken = default)
    {
        var result = await _gameEngine.ExecuteRoundAsync(session.Level, session.Players, session.PendingActions, session.Mobs, cancellationToken);

        await session.LockAsync(_ =>
        {
            session.Players = [.. result.Players];
            session.Mobs = [.. result.Mobs];
            session.LastActivity = DateTimeOffset.UtcNow;
            session.PendingActions.Clear();

            return Task.CompletedTask;
        }, cancellationToken);

        return result;
    }

    /// <summary>
    ///     Ends and removes the session for the given channel, persists player progress, and returns summary results.
    /// </summary>
    /// <param name="channelId">Channel id of the session to end.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="SessionEndResult" /> containing the ended session and any level-ups.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the session could not be removed.</exception>
    public async Task<SessionEndResult> EndSessionAsync(ulong channelId, CancellationToken cancellationToken = default)
    {
        if (!Sessions.TryRemove(channelId, out var session))
        {
            throw new InvalidOperationException("Could not remove game session.");
        }

        var allLevelUps = new Dictionary<PlayerId, List<SkillLevelUp>>();

        // Save progress for all human players
        var humanPlayers = session.Players.Where(p => !p.IsBot).ToList();
        foreach (var player in humanPlayers)
        {
            try
            {
                // Determine if player died (not alive at session end)
                var died = !player.IsAlive;
                var result = await _playerService.SaveProgressAsync(player, session.Level, died, cancellationToken);

                if (result.LevelUps.Count > 0)
                {
                    allLevelUps[player.Id] = result.LevelUps;
                    foreach (var levelUp in result.LevelUps)
                    {
                        _logger.LogInformation(
                            "Player {PlayerName} leveled up {Skill}: {OldLevel} -> {NewLevel}",
                            player.Name, levelUp.SkillName, levelUp.OldLevel, levelUp.NewLevel);
                    }
                }

                _logger.LogDebug(
                    "Saved progress for player {PlayerId}: Gold={Gold}, AttackXp={AttackXp}, MagicXp={MagicXp}, SpeechXp={SpeechXp}, DefenseXp={DefenseXp}, Died={Died}",
                    player.Id.Value, player.GoldEarned, player.AttackXpEarned, player.MagicXpEarned, player.SpeechXpEarned, player.DefenseXpEarned, died);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save progress for player {PlayerId}", player.Id.Value);
            }
        }

        _logger.LogInformation("Ended session in channel {ChannelId}. Saved progress for {PlayerCount} players.", channelId, humanPlayers.Count);

        return new SessionEndResult(session, allLevelUps);
    }
}