using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Actions;
using Libertas.Discord.Adventure.Core.Services.Combat;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using Moq;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests.Actions;

[TestFixture]
public class MagicActionHandlerTests
{
    [Test]
    public void Execute_AwardsXpAndAppliesDamage()
    {
        var calc = TestServiceFactory.CreateCombatCalculator();
        var loc = TestServiceFactory.CreateLocalizationService();
        var progression = TestServiceFactory.CreateProgressionService();

        var xpMock = new Mock<IXpDistributor>();
        var lootMock = new Mock<ILootDistributor>();
        var damageMock = new Mock<IDamageApplier>();

        var handler = new MagicActionHandler(calc, loc, progression, xpMock.Object, lootMock.Object, damageMock.Object);

        var player = TestEntityFactory.CreatePlayer("Mage", attackPower: 5, magicPower: 10);
        var mob = TestEntityFactory.CreateWeakMob("Imp");

        var players = new List<PlayerState> { player };
        var mobs = new List<MobState> { mob };
        var messages = new List<string>();

        var context = new CombatContext<PlayerState>(1, player, players, mobs, messages);

        damageMock.Setup(d => d.ApplyDamageToMob(mob, It.IsAny<int>(), context)).Returns(5);

        handler.Execute(context);

        xpMock.Verify(x => x.AwardSkillXp(player, SkillType.Magic, 1), Times.Once);
        damageMock.Verify(d => d.ApplyDamageToMob(mob, It.IsAny<int>(), context), Times.Once);
    }
}