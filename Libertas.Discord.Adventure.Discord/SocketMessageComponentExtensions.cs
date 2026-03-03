using Discord.WebSocket;
using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Discord;

/// <summary>
/// Extension methods for parsing Discord component custom IDs for player actions.
/// </summary>
public static class SocketMessageComponentExtensions
{
    /// <summary>
    /// Attempts to parse an action custom id from the Discord component.
    /// Expected format: "action-{actionValue}-{channelId}".
    /// </summary>
    /// <param name="component">Discord message component to parse.</param>
    /// <param name="action">Parsed player action enum value.</param>
    /// <param name="channelId">Parsed channel ID.</param>
    /// <returns>True if parsing succeeded; otherwise false.</returns>
    public static bool TryParseActionId(this SocketMessageComponent? component, out PlayerAction action, out ulong channelId)
    {
        try
        {
            action = default;
            channelId = 0;

            var customId = component?.Data?.CustomId;

            // Validate custom ID format
            if (string.IsNullOrWhiteSpace(customId))
            {
                return false;
            }

            var parts = customId.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
            {
                return false;
            }

            if (!string.Equals(parts[0], "action", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // action part expected to be an integer enum value
            if (!int.TryParse(parts[1], out var actionVal))
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(PlayerAction), actionVal))
            {
                return false;
            }

            action = (PlayerAction)actionVal;

            // Parse channel ID
            if (!ulong.TryParse(parts[2], out channelId))
            {
                return false;
            }

            return true;
        }
        catch
        {
            action = default;
            channelId = 0;
            return false;
        }
    }
}