using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Phases;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests.Phases;

[TestFixture]
public class RoundSummaryGeneratorTests
{
    private RoundSummaryGenerator _generator = null!;

    [SetUp]
    public void SetUp()
    {
        _generator = new RoundSummaryGenerator();
    }

    [Test]
    public void AddSummary_ShouldGenerateSummary_ForPlayersWhoEarnedRewards()
    {
        // Arrange
        var player = new PlayerState { Id = new PlayerId(1), Name = "Player1", GoldEarned = 100, MobsKilled = 1, AttackXpEarned = 50 };
        var earningsBefore = new Dictionary<PlayerId, RoundSummaryGenerator.EarningsSnapshot>
        {
            { player.Id, new RoundSummaryGenerator.EarningsSnapshot(50, 0, 25, 0, 0, 0) }
        };
        var messages = new List<string>();

        // Act
        RoundSummaryGenerator.AddSummary([player], earningsBefore, messages);

        // Assert
        Assert.That(messages.Count, Is.EqualTo(3)); // Blank line, "Round Earnings:", and the summary
        Assert.That(messages[1], Is.EqualTo("Round Earnings:"));
        Assert.That(messages[2], Does.Contain("Player1: 1 kill"));
        Assert.That(messages[2], Does.Contain("+25 ATK"));
    }

    [Test]
    public void AddSummary_ShouldSkipPlayersWhoEarnedNothing()
    {
        // Arrange
        var player = new PlayerState { Id = new PlayerId(1), Name = "Player1", GoldEarned = 50, MobsKilled = 0, AttackXpEarned = 25 };
        var earningsBefore = new Dictionary<PlayerId, RoundSummaryGenerator.EarningsSnapshot>
        {
            { player.Id, new RoundSummaryGenerator.EarningsSnapshot(50, 0, 25, 0, 0, 0) }
        };
        var messages = new List<string>();

        // Act
        RoundSummaryGenerator.AddSummary([player], earningsBefore, messages);

        // Assert
        Assert.That(messages, Is.Empty);
    }
}