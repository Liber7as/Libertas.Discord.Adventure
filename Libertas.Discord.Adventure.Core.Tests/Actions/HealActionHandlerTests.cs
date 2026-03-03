using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Actions;
using Libertas.Discord.Adventure.Core.Services.Combat;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using Moq;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests.Actions;

[TestFixture]
public class HealActionHandlerTests
{
    [Test]
    public void Execute_AwardsMagicXpAndHeals()
    {
        var calc = TestServiceFactory.CreateCombatCalculator();
        var loc = TestServiceFactory.CreateLocalizationService();
        var progression = TestServiceFactory.CreateProgressionService();

        var xpMock = new Mock<IXpDistributor>();

        var handler = new HealActionHandler(calc, loc, progression, xpMock.Object);

        var healer = TestEntityFactory.CreatePlayer("Healer", magicPower: 10);
        var injured = TestEntityFactory.CreatePlayer("Wounded", maxHp: 30);
        injured.CurrentHp = 10;

        var players = new List<PlayerState> { healer, injured };
        var mobs = new List<MobState> { TestEntityFactory.CreateWeakMob("Goblin") };
        var messages = new List<string>();

        var context = new CombatContext<PlayerState>(1, healer, players, mobs, messages);

        handler.Execute(context);

        xpMock.Verify(x => x.AwardSkillXp(healer, SkillType.Magic, 1), Times.Once);
        Assert.That(injured.CurrentHp, Is.GreaterThan(10));
    }
}
