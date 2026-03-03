using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Combat;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests.Combat;

[TestFixture]
public class XpDistributorTests
{
    [Test]
    public void AwardSkillXp_DoesNotAwardForBots()
    {
        var progression = TestServiceFactory.CreateProgressionService();
        var distributor = new XpDistributor(progression);

        var bot = new PlayerState { Id = new PlayerId(1), IsBot = true };

        distributor.AwardSkillXp(bot, SkillType.Attack, 1);

        Assert.That(bot.AttackXpEarned, Is.EqualTo(0));
    }

    [Test]
    public void AwardSkillXp_IncrementsHumanXp()
    {
        var progression = TestServiceFactory.CreateProgressionService();
        var distributor = new XpDistributor(progression);

        var player = new PlayerState { Id = new PlayerId(2), IsBot = false };

        distributor.AwardSkillXp(player, SkillType.Attack, 1);

        Assert.That(player.AttackXpEarned, Is.GreaterThan(0));
    }
}