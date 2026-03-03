using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Actions;
using Libertas.Discord.Adventure.Core.Services.Combat;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using Moq;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests.Actions;

[TestFixture]
public class AttackActionHandlerTests
{
    [Test]
    public void Execute_UsesDistributorsAndApplier()
    {
        var calc = TestServiceFactory.CreateCombatCalculator();
        var loc = TestServiceFactory.CreateLocalizationService();
        var progression = TestServiceFactory.CreateProgressionService();

        var xpMock = new Mock<IXpDistributor>();
        var lootMock = new Mock<ILootDistributor>();
        var damageMock = new Mock<IDamageApplier>();

        var handler = new AttackActionHandler(calc, loc, progression, xpMock.Object, lootMock.Object, damageMock.Object);

        var player = new PlayerState { Id = new PlayerId(1), Name = "Attacker", IsBot = false, AttackPower = new PowerLevel(5), CurrentHp = 20 };
        var mob = new MobState { Name = "Goblin", CurrentHp = 10, AttackPower = new PowerLevel(3) };

        var players = new List<PlayerState> { player };
        var mobs = new List<MobState> { mob };
        var messages = new List<string>();

        var context = new CombatContext<PlayerState>(1, player, players, mobs, messages);

        damageMock.Setup(d => d.ApplyDamageToMob(mob, It.IsAny<int>(), context)).Returns(5);

        handler.Execute(context);

        xpMock.Verify(x => x.AwardSkillXp(player, SkillType.Attack, 1), Times.Once);
        damageMock.Verify(d => d.ApplyDamageToMob(mob, It.IsAny<int>(), context), Times.Once);
    }
}
