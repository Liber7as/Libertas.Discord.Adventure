using System.ComponentModel.DataAnnotations;

namespace Libertas.Discord.Adventure.Discord;

/// <summary>
///     Configuration settings for Discord integration. Bound from configuration (e.g. "Discord" section).
/// </summary>
public sealed class DiscordSettings
{
    /// <summary>
    ///     Bot token used to login the Discord client.
    /// </summary>
    [Required]
    public required string Token { get; init; }

    /// <summary>
    ///     The prefix used for commands (e.g. !admin say hello).
    /// </summary>
    [Required]
    public string Prefix { get; set; } = "!";
}