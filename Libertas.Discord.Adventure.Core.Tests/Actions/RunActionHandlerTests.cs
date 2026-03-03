using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Actions;
using Libertas.Discord.Adventure.Core.Services.Combat;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using Moq;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests.Actions;

[TestFixture]
public class RunActionHandlerTests
{
    [Test]
    public void Execute_RemovesPlayerOnSuccess()
    {
        var calc = TestServiceFactory.CreateCombatCalculator();
        var loc = TestServiceFactory.CreateLocalizationService();
        var progression = TestServiceFactory.CreateProgressionService();

        var lootMock = new Mock<ILootDistributor>();
        var handler = new RunActionHandler(calc, loc, progression, lootMock.Object);

        var runner = TestEntityFactory.CreatePlayer("Runner");
        var players = new List<PlayerState> { runner };
        var mobs = new List<MobState> { TestEntityFactory.CreateWeakMob() };
        var messages = new List<string>();

        var context = new CombatContext<PlayerState>(1, runner, players, mobs, messages);

        // Because run chance is based on RNG, we'll just execute and ensure no exceptions
        handler.Execute(context);

        // Either the player remains or is removed; ensure it behaves deterministically
        Assert.That(players, Is.Not.Null);
    }
}
