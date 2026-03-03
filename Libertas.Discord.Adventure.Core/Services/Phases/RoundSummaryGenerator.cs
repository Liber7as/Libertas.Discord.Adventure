using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Phases;

/// <summary>
///     Generates the round summary, including XP and gold earned by players.
/// </summary>
public class RoundSummaryGenerator
{
    /// <summary>
    ///     Adds a round summary message showing XP and gold earned this round for each human player.
    ///     Only includes players who earned something during the round.
    /// </summary>
    public static void AddSummary(
        List<PlayerState> players,
        Dictionary<PlayerId, EarningsSnapshot> earningsBefore,
        List<string> messages)
    {
        var summaryLines = new List<string>();

        foreach (var player in players.Where(p => !p.IsBot))
        {
            if (!earningsBefore.TryGetValue(player.Id, out var before))
            {
                continue;
            }

            // Calculate deltas for this round
            var goldThisRound = player.GoldEarned - before.Gold;
            var killsThisRound = player.MobsKilled - before.Kills;
            var attackXpThisRound = player.AttackXpEarned - before.AttackXp;
            var magicXpThisRound = player.MagicXpEarned - before.MagicXp;
            var speechXpThisRound = player.SpeechXpEarned - before.SpeechXp;
            var defenseXpThisRound = player.DefenseXpEarned - before.DefenseXp;

            var totalXp = attackXpThisRound + magicXpThisRound + speechXpThisRound + defenseXpThisRound;

            // Skip players who earned nothing
            if (goldThisRound <= 0 && totalXp <= 0 && killsThisRound <= 0)
            {
                continue;
            }

            // Build earnings summary parts
            var parts = new List<string>();

            if (killsThisRound > 0)
            {
                parts.Add($"{killsThisRound} kill{(killsThisRound > 1 ? "s" : "")}");
            }

            if (goldThisRound > 0)
            {
                parts.Add($"{goldThisRound:F0}g");
            }

            if (attackXpThisRound > 0)
            {
                parts.Add($"+{attackXpThisRound} ATK");
            }

            if (magicXpThisRound > 0)
            {
                parts.Add($"+{magicXpThisRound} MAG");
            }

            if (speechXpThisRound > 0)
            {
                parts.Add($"+{speechXpThisRound} SPE");
            }

            if (defenseXpThisRound > 0)
            {
                parts.Add($"+{defenseXpThisRound} DEF");
            }

            if (parts.Count > 0)
            {
                summaryLines.Add($"  {player.Name}: {string.Join(", ", parts)}");
            }
        }

        if (summaryLines.Count > 0)
        {
            messages.Add(string.Empty); // Blank line separator
            messages.Add("Round Earnings:");
            messages.AddRange(summaryLines);
        }
    }

    /// <summary>
    ///     Snapshot of a player's earnings at a point in time, used for calculating round deltas.
    /// </summary>
    public readonly record struct EarningsSnapshot(
        double Gold,
        int Kills,
        long AttackXp,
        long MagicXp,
        long SpeechXp,
        long DefenseXp);
}