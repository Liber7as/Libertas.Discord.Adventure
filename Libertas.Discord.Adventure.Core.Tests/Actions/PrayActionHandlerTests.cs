using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Actions;
using Libertas.Discord.Adventure.Core.Services.Combat;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using Moq;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests.Actions;

[TestFixture]
public class PrayActionHandlerTests
{
    [Test]
    public void Execute_AwardsXpAndHandlesSmiteOrHeal()
    {
        var calc = TestServiceFactory.CreateCombatCalculator();
        var loc = TestServiceFactory.CreateLocalizationService();
        var progression = TestServiceFactory.CreateProgressionService();

        var xpMock = new Mock<IXpDistributor>();
        var lootMock = new Mock<ILootDistributor>();
        var damageMock = new Mock<IDamageApplier>();

        var handler = new PrayActionHandler(calc, loc, progression, xpMock.Object, lootMock.Object, damageMock.Object);

        var player = TestEntityFactory.CreatePlayer("Cleric", magicPower: 10);
        var mob = TestEntityFactory.CreateWeakMob("Imp");

        var players = new List<PlayerState> { player };
        var mobs = new List<MobState> { mob };
        var messages = new List<string>();

        var context = new CombatContext<PlayerState>(1, player, players, mobs, messages);

        handler.Execute(context);

        xpMock.Verify(x => x.AwardSkillXp(player, SkillType.Magic, 1), Times.Once);
    }
}