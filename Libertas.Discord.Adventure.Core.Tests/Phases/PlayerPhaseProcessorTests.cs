using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Core.Services.Actions;
using Libertas.Discord.Adventure.Core.Services.Phases;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Moq; // Added for mocking dependencies

namespace Libertas.Discord.Adventure.Core.Tests.Phases;

[TestFixture]
public class PlayerPhaseProcessorTests
{
    private PlayerPhaseProcessor _processor = null!;
    private Mock<IBotService> _botServiceMock = null!;
    private Mock<IActionResolutionService> _actionResolutionServiceMock = null!;

    [SetUp]
    public void SetUp()
    {
        _botServiceMock = new Mock<IBotService>();
        _actionResolutionServiceMock = new Mock<IActionResolutionService>();
        _processor = new PlayerPhaseProcessor(
            _botServiceMock.Object,
            _actionResolutionServiceMock.Object,
            NullLogger<PlayerPhaseProcessor>.Instance);
    }

    [Test]
    public void Execute_ShouldHandlePlayerActions()
    {
        // Arrange
        var player = new PlayerState { Id = new PlayerId(1), Name = "Player1", IsBot = false, CurrentHp = 10 };
        var mob = new MobState { Name = "Mob1", CurrentHp = 20 };
        var playerList = new List<PlayerState> { player };
        var mobList = new List<MobState> { mob };
        var playerActions = new Dictionary<PlayerId, PlayerAction> { { player.Id, PlayerAction.Attack } };
        var messages = new List<string>();

        // Act
        _processor.Execute(playerList, mobList, playerActions, 1, messages);

        // Assert
        _actionResolutionServiceMock.Verify(
            x => x.HandlePlayerAction(
                It.Is<CombatContext<PlayerState>>(c => c.Actor == player && c.Level == 1),
                PlayerAction.Attack),
            Times.Once);
    }

    [Test]
    public void Execute_ShouldHandleBotActions()
    {
        // Arrange
        var bot = new PlayerState { Id = new PlayerId(2), Name = "Bot1", IsBot = true, CurrentHp = 10 };
        var mob = new MobState { Name = "Mob1", CurrentHp = 20 };
        var playerList = new List<PlayerState> { bot };
        var mobList = new List<MobState> { mob };
        var messages = new List<string>();

        _botServiceMock.Setup(x => x.DecideBotAction(bot, It.IsAny<IReadOnlyList<PlayerState>>(), It.IsAny<IReadOnlyList<MobState>>())).Returns(PlayerAction.Magic);

        // Act
        _processor.Execute(playerList, mobList, new Dictionary<PlayerId, PlayerAction>(), 1, messages);

        // Assert
        _actionResolutionServiceMock.Verify(
            x => x.HandlePlayerAction(
                It.Is<CombatContext<PlayerState>>(c => c.Actor == bot && c.Level == 1),
                PlayerAction.Magic),
            Times.Once);
    }
}