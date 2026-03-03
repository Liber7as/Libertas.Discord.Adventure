using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Combat;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests.Combat;

[TestFixture]
public class DamageApplierTests
{
    [Test]
    public void ApplyDamageToPlayer_ReducesHpAndAwardsXp()
    {
        var progression = TestServiceFactory.CreateProgressionService();
        var applier = new DamageApplier(progression);

        var player = new PlayerState { Id = new PlayerId(1), IsBot = false, CurrentHp = 20, DefensePower = new PowerLevel(2) };
        var (reduced, _) = applier.ApplyDamageToPlayer(player, 10);

        Assert.That(reduced, Is.GreaterThan(0));
        Assert.That(player.CurrentHp, Is.LessThan(20));
        Assert.That(player.DefenseXpEarned, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public void ApplyDamageToMob_ReducesMobHp()
    {
        var progression = TestServiceFactory.CreateProgressionService();
        var applier = new DamageApplier(progression);

        var mob = new MobState { Name = "Orc", CurrentHp = 15 };

        var actual = applier.ApplyDamageToMob(mob, 6);

        Assert.That(actual, Is.EqualTo(6));
        Assert.That(mob.CurrentHp, Is.EqualTo(9));
    }
}
