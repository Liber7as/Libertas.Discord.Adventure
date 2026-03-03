using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Core.Services.Phases;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests.Phases;

[TestFixture]
public class BotPhaseProcessorTests
{
    [SetUp]
    public void SetUp()
    {
        _botServiceMock = new Mock<IBotService>();
        _processor = new BotPhaseProcessor(
            _botServiceMock.Object,
            NullLogger<BotPhaseProcessor>.Instance);
    }

    private BotPhaseProcessor _processor = null!;
    private Mock<IBotService> _botServiceMock = null!;

    [Test]
    public void InjectBotsIfNeeded_ShouldAddBots_WhenPartyIsBelowMinimumSize()
    {
        // Arrange
        var playerList = new List<PlayerState> { new() { Id = new PlayerId(1), Name = "Player1", IsBot = false } };
        var generatedBots = new List<PlayerState> { new() { Id = new PlayerId(2), Name = "Bot1", IsBot = true } };

        _botServiceMock.Setup(x => x.GenerateBotsForParty(It.IsAny<IReadOnlyList<PlayerState>>(), 1)).Returns(generatedBots);

        // Act
        var result = _processor.InjectBotsIfNeeded(playerList, 1);

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(playerList.Count, Is.EqualTo(2));
        Assert.That(playerList, Has.Member(generatedBots[0]));
    }

    [Test]
    public void InjectBotsIfNeeded_ShouldNotAddBots_WhenPartyMeetsMinimumSize()
    {
        // Arrange
        var playerList = new List<PlayerState> { new() { Id = new PlayerId(1), Name = "Player1", IsBot = false } };

        _botServiceMock.Setup(x => x.GenerateBotsForParty(It.IsAny<IReadOnlyList<PlayerState>>(), 1)).Returns([]);

        // Act
        var result = _processor.InjectBotsIfNeeded(playerList, 1);

        // Assert
        Assert.That(result.Count, Is.EqualTo(0));
        Assert.That(playerList.Count, Is.EqualTo(1));
    }
}