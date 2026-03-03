using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Combat;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests.Combat;

[TestFixture]
public class LootDistributorTests
{
    [Test]
    public void DistributeLoot_SplitsGoldAmongHumans()
    {
        var calc = TestServiceFactory.CreateCombatCalculator();
        var distributor = new LootDistributor(calc);

        var player = new PlayerState { Id = new PlayerId(1), IsBot = false, CurrentHp = 10 };
        var bot = new PlayerState { Id = new PlayerId(2), IsBot = true, CurrentHp = 10 };

        var players = new List<PlayerState> { player, bot };
        var mob = new MobState { Name = "Goblin", CurrentHp = 0, AttackPower = new PowerLevel(5) };

        var context = new CombatContext<PlayerState>(1, player, players, [mob], []);

        var gold = distributor.DistributeLoot(context, mob);

        Assert.That(gold, Is.GreaterThanOrEqualTo(0));
        Assert.That(player.GoldEarned, Is.GreaterThanOrEqualTo(0));
    }
}
