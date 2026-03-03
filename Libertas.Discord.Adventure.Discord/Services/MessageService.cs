using System.Text;
using Discord;
using Discord.WebSocket;
using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Microsoft.Extensions.Logging;

namespace Libertas.Discord.Adventure.Discord.Services;

/// <summary>
///     Helper for building and sending adventure-related Discord messages.
///     Encapsulates logic for creating combat components, posting round information, and updating the
///     persistent combat message used to collect player actions.
/// </summary>
/// <remarks>
///     This class resolves the target channel from an <see cref="AdventureSession" /> and is intended
///     to be used from background services and event handlers. Methods do not throw on missing channels
///     and instead log warnings to keep Discord event handling resilient.
/// </remarks>
public class MessageService(DiscordSocketClient client, ILogger<MessageService> logger, IActionLocalizationService localization)
{
    private readonly DiscordSocketClient _client = client ?? throw new ArgumentNullException(nameof(client));
    private readonly IActionLocalizationService _localization = localization ?? throw new ArgumentNullException(nameof(localization));
    private readonly ILogger<MessageService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Sends an initial round information message to the session channel containing a teaser,
    ///     mob details, and the next-round timestamp.
    /// </summary>
    /// <param name="session">Session containing channel and mob data.</param>
    /// <param name="round">Round number within the current level.</param>
    /// <param name="nextRound">Timestamp for when the next round will begin.</param>
    public async Task SendRoundInformationAsync(AdventureSession session, int round, DateTimeOffset nextRound)
    {
        var channel = GetChannel(session);

        if (channel == null)
        {
            _logger.LogWarning("Could not resolve channel {ChannelId} for initial prompt.", session.ChannelId);
            return;
        }

        // Build a single component containing teaser, mob details and the timestamp to avoid message spam
        var component = BuildRoundInformationComponent(session, round, nextRound);

        await channel.SendMessageAsync(components: component.Build());
    }

    /// <summary>
    ///     Updates the existing persistent combat message's button components to reflect current
    ///     action counts from <see cref="AdventureSession.PendingActions" />.
    /// </summary>
    public async Task UpdateCombatButtonsAsync(AdventureSession session)
    {
        var channel = GetChannel(session);
        if (channel == null)
        {
            _logger.LogWarning("Could not resolve channel {ChannelId} for updating combat buttons.", session.ChannelId);
            return;
        }

        if (session.CombatMessageId == null)
        {
            _logger.LogWarning("No combat message stored for channel {ChannelId}.", session.ChannelId);
            return;
        }


        if (await channel.GetMessageAsync(session.CombatMessageId.Value) is not IUserMessage msg)
        {
            _logger.LogWarning("Could not retrieve combat message {MessageId} in channel {ChannelId}.", session.CombatMessageId, session.ChannelId);
            return;
        }

        var component = BuildCombatComponent(session);

        await msg.ModifyAsync(m => m.Components = component.Build());
    }

    /// <summary>
    ///     Sends the combat action buttons as a new message for the session and stores the message id
    ///     on the provided session so it can be updated later.
    /// </summary>
    /// <summary>
    ///     Sends a new combat message containing action buttons for the session and stores the message id
    ///     on the session for subsequent updates.
    /// </summary>
    public async Task SendCombatMessageAsync(AdventureSession session)
    {
        var channel = GetChannel(session);
        if (channel == null)
        {
            _logger.LogWarning("Could not resolve channel {ChannelId} when sending combat message.", session.ChannelId);
            return;
        }

        var component = BuildCombatComponent(session);

        var sent = await channel.SendMessageAsync(components: component.Build());

        if (sent == null)
        {
            _logger.LogWarning("Failed to send combat message in channel {ChannelId}.", session.ChannelId);
            return;
        }

        // Store the combat message id on the session for later updates
        session.CombatMessageId = sent.Id;
    }

    /// <summary>
    ///     Deletes the session's stored combat message if one exists and clears the id.
    ///     This is safe to call even if the id is null.
    /// </summary>
    /// <summary>
    ///     Deletes and clears the session's stored combat message id. Safe to call if no id exists.
    /// </summary>
    public async Task DeleteCombatMessageAsync(AdventureSession session)
    {
        var channel = GetChannel(session);

        if (channel == null)
        {
            _logger.LogWarning("Could not resolve channel {ChannelId} when deleting combat message.", session.ChannelId);
            return;
        }

        if (session.CombatMessageId == null)
        {
            // Nothing to delete
            return;
        }

        try
        {
            var msg = await channel.GetMessageAsync(session.CombatMessageId.Value);

            if (msg != null)
            {
                await msg.DeleteAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete combat message {MessageId} in channel {ChannelId}.", session.CombatMessageId, session.ChannelId);
        }
        finally
        {
            session.CombatMessageId = null;
        }
    }

    /// <summary>
    ///     Rebuilds the combat component (refreshing the per-action counts) and updates the existing
    ///     combat message stored on the session. This is intentionally the same behavior as
    ///     UpdateCombatButtonsAsync but provided under the name expected by the event handler.
    /// </summary>
    /// <summary>
    ///     Rebuilds and applies the combat component to the stored combat message. Provided for
    ///     compatibility with event handler naming.
    /// </summary>
    public async Task UpdateCombatMessageAsync(AdventureSession session)
    {
        var channel = GetChannel(session);

        if (channel == null)
        {
            _logger.LogWarning("Could not resolve channel {ChannelId} for updating combat message.", session.ChannelId);
            return;
        }

        if (session.CombatMessageId == null)
        {
            _logger.LogWarning("No combat message stored for channel {ChannelId}.", session.ChannelId);
            return;
        }


        if (await channel.GetMessageAsync(session.CombatMessageId.Value) is not IUserMessage msg)
        {
            _logger.LogWarning("Could not retrieve combat message {MessageId} in channel {ChannelId}.", session.CombatMessageId, session.ChannelId);
            return;
        }

        var component = BuildCombatComponent(session);

        await msg.ModifyAsync(m => m.Components = component.Build());
    }

    /// <summary>
    ///     Posts the compact combat log and a status summary for the completed round to the session channel.
    /// </summary>
    /// <param name="session">The session the round belongs to.</param>
    /// <param name="result">Round result including player/mob states and messages.</param>
    /// <param name="nextRound">Timestamp for the next round.</param>
    public async Task PostRoundResultsAsync(AdventureSession session, RoundResult result, DateTimeOffset nextRound)
    {
        var channel = GetChannel(session);

        if (channel == null)
        {
            _logger.LogWarning("Could not resolve channel {ChannelId} for posting round results.", session.ChannelId);
            return;
        }

        var sb = new StringBuilder();

        // --- Combat Log (compact) ---
        // Only include non-empty lines and exclude the round earnings header for clarity
        var combatMessages = result.Messages
            .Where(m => !string.IsNullOrWhiteSpace(m) && m != "📊 Round Earnings:")
            .ToList();

        if (combatMessages.Count > 0)
        {
            sb.AppendLine("```");
            sb.Append(string.Join(Environment.NewLine, combatMessages));
            sb.AppendLine();
            sb.AppendLine("```");
        }

        // --- Status Bar (one line per entity) ---
        var aliveHumans = result.Players.Where(p => !p.IsBot && p.IsAlive).ToList();
        var deadHumans = result.Players.Where(p => !p.IsBot && !p.IsAlive).ToList();
        var aliveBots = result.Players.Where(p => p.IsBot && p.IsAlive).ToList();
        var aliveMobs = result.Mobs.Where(m => m.IsAlive).ToList();

        // Player HP bar
        var playerParts = new List<string>();
        foreach (var p in aliveHumans)
        {
            playerParts.Add($"{p.Name}: {p.CurrentHp}hp");
        }

        foreach (var b in aliveBots)
        {
            playerParts.Add($"{b.Name}🤖: {b.CurrentHp}hp");
        }

        foreach (var d in deadHumans)
        {
            playerParts.Add($"~~{d.Name}~~💀");
        }

        if (playerParts.Count > 0)
        {
            sb.AppendLine($"**Party** — {string.Join(", ", playerParts)}");
        }

        // Mob HP bar
        if (aliveMobs.Count > 0)
        {
            var mobParts = aliveMobs.Select(m => $"{m.Name}: {m.CurrentHp}/{m.MaxHp}hp").ToList();
            sb.AppendLine($"**Mobs** — {string.Join(", ", mobParts)}");
        }
        else
        {
            sb.AppendLine("**All mobs defeated!** 🎉");
        }

        // Next round timestamp
        sb.AppendLine($"-# Next round <t:{nextRound.ToUnixTimeSeconds()}:R>");

        await channel.SendMessageAsync(sb.ToString());
    }

    private ISocketMessageChannel? GetChannel(AdventureSession session)
    {
        return _client.GetChannel(session.ChannelId) as ISocketMessageChannel;
    }

    private ComponentBuilderV2 BuildRoundInformationComponent(AdventureSession session, int round, DateTimeOffset nextRound)
    {
        var builder = new ComponentBuilderV2();

        // Teaser text
        var teaserBuilder = new StringBuilder();
        teaserBuilder.AppendLine($"# Level {session.Level}, Round {round}");

        // 1st round is the teaser for the whole level
        if (round == 1)
        {
            teaserBuilder.AppendLine($"## {_localization.GetTeaserMessage()}");
        }

        builder = builder.WithTextDisplay(teaserBuilder.ToString());

        // Reuse the mob component container
        var mobContainer = BuildMobComponent(session);
        builder = builder.WithContainer(mobContainer);

        // Timestamp
        var timestampTag = $"<t:{nextRound.ToUnixTimeSeconds()}:R>";
        builder = builder.WithTextDisplay($"Round results: {timestampTag}");

        return builder;
    }

    private static ContainerBuilder BuildMobComponent(AdventureSession session)
    {
        var container = new ContainerBuilder();

        if (session.Mobs.Count == 0)
        {
            return container.WithTextDisplay("No mobs present.");
        }

        foreach (var mob in session.Mobs)
        {
            var mobDetails = new StringBuilder();
            mobDetails.AppendLine($"# {mob.Name}");
            mobDetails.AppendLine($"HP: `{mob.CurrentHp}/{mob.MaxHp}`");

            container = container.WithTextDisplay(mobDetails.ToString());

            if (!string.IsNullOrWhiteSpace(mob.ImageUrl))
            {
                container = container.WithMediaGallery(new[] { mob.ImageUrl });
            }
        }

        return container;
    }

    private static ComponentBuilderV2 BuildCombatComponent(AdventureSession session)
    {
        var attackCount = session.Players.Count(p => p.IsAlive && session.PendingActions.TryGetValue(p.Id, out var a) && a == PlayerAction.Attack);
        var magicCount = session.Players.Count(p => p.IsAlive && session.PendingActions.TryGetValue(p.Id, out var a) && a == PlayerAction.Magic);
        var healCount = session.Players.Count(p => p.IsAlive && session.PendingActions.TryGetValue(p.Id, out var a) && a == PlayerAction.Heal);
        var talkCount = session.Players.Count(p => p.IsAlive && session.PendingActions.TryGetValue(p.Id, out var a) && a == PlayerAction.Talk);
        var prayCount = session.Players.Count(p => p.IsAlive && session.PendingActions.TryGetValue(p.Id, out var a) && a == PlayerAction.Pray);
        var runCount = session.Players.Count(p => p.IsAlive && session.PendingActions.TryGetValue(p.Id, out var a) && a == PlayerAction.Run);

        var builder = new ComponentBuilderV2();

        var buttons = new[]
        {
            new ButtonBuilder(LabelWithCount("Fight", attackCount), $"action-{(int)PlayerAction.Attack}-{session.ChannelId}").WithEmote(new Emoji("⚔️")),
            new ButtonBuilder(LabelWithCount("Magic", magicCount), $"action-{(int)PlayerAction.Magic}-{session.ChannelId}").WithEmote(new Emoji("✨")),
            new ButtonBuilder(LabelWithCount("Heal", healCount), $"action-{(int)PlayerAction.Heal}-{session.ChannelId}").WithEmote(new Emoji("💚")),
            new ButtonBuilder(LabelWithCount("Talk", talkCount), $"action-{(int)PlayerAction.Talk}-{session.ChannelId}").WithEmote(new Emoji("💬")),
            new ButtonBuilder(LabelWithCount("Pray", prayCount), $"action-{(int)PlayerAction.Pray}-{session.ChannelId}").WithEmote(new Emoji("🙏")),
            new ButtonBuilder(LabelWithCount("Run", runCount), $"action-{(int)PlayerAction.Run}-{session.ChannelId}").WithEmote(new Emoji("🏃"))
        };

        // Discord limits an action row to 5 components. Add buttons in chunks of up to 5 per row.
        const int maxPerRow = 5;
        for (var i = 0; i < buttons.Length; i += maxPerRow)
        {
            // Add up to 5 buttons per row to comply with Discord's UI constraints
            var chunk = buttons.Skip(i).Take(maxPerRow).ToArray();
            builder = builder.WithActionRow(chunk);
        }

        return builder;

        static string LabelWithCount(string label, int count)
        {
            return count == 0 ? label : $"{label} ({count})";
        }
    }
}