using Discord.Commands;

namespace Libertas.Discord.Adventure.Discord.Modules;

// https://docs.discordnet.dev/guides/text_commands/intro.html#modules

/// <summary>
///     Discord command module for admin-only commands.
/// </summary>
[Group("admin")]
[RequireOwner]
public class AdminModule : ModuleBase<SocketCommandContext>
{
    /// <summary>
    ///     Echoes the provided message back to the channel.
    /// </summary>
    /// <param name="text">Message to echo.</param>
    [Command("say")]
    [Summary("Echoes a message.")]
    public async Task SayAsync([Remainder] string text)
    {
        await ReplyAsync(text);
    }
}