using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Core.Services.Actions;
using Libertas.Discord.Adventure.Core.Services.Phases;
using NUnit.Framework;
using Moq; // Added for mocking dependencies

namespace Libertas.Discord.Adventure.Core.Tests.Phases;

[TestFixture]
public class MobPhaseProcessorTests
{
    private MobPhaseProcessor _processor = null!;
    private Mock<IActionResolutionService> _actionResolutionServiceMock = null!;

    [SetUp]
    public void SetUp()
    {
        _actionResolutionServiceMock = new Mock<IActionResolutionService>();
        _processor = new MobPhaseProcessor(_actionResolutionServiceMock.Object);
    }

    [Test]
    public void Execute_ShouldHandleMobActions()
    {
        // Arrange
        var mob = new MobState { Name = "Mob1", CurrentHp = 20 };
        var player = new PlayerState { Id = new PlayerId(1), Name = "Player1", CurrentHp = 10 };
        var mobList = new List<MobState> { mob };
        var playerList = new List<PlayerState> { player };
        var messages = new List<string>();

        // Act
        _processor.Execute(playerList, mobList, 1, messages);

        // Assert
        _actionResolutionServiceMock.Verify(
            x => x.HandleMobAction(
                It.Is<CombatContext<MobState>>(c => c.Actor == mob && c.Level == 1)),
            Times.Once);
    }
}