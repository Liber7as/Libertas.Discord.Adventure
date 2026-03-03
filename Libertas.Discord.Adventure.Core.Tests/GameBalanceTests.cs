using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests;

/// <summary>
///     Tests for game balance and fairness criteria.
///     These tests validate that the game feels fair and challenging without being impossible.
///     Fairness Criteria:
///     1. Critical hit rate should be approximately 15%
///     2. Prayer success rate should scale with level (easier at low levels)
///     3. Mob damage should scale reasonably (not one-shot at low levels)
///     4. Defense should meaningfully reduce damage
///     5. Healing should be impactful but not overpowered
/// </summary>
[TestFixture]
[Category("Balance")]
public class GameBalanceTests
{
    [SetUp]
    public void SetUp()
    {
        TestEntityFactory.ResetIdCounters();
    }

    /// <summary>
    ///     Validates that critical hits occur at approximately 15% rate.
    ///     Uses statistical sampling over many iterations.
    /// </summary>
    [Test]
    public async Task CritRate_ApproximatelyFifteenPercent()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var critCount = 0;
        var totalAttacks = 0;
        const int iterations = 500;

        // Act
        for (var i = 0; i < iterations; i++)
        {
            var player = TestEntityFactory.CreateWarrior($"Attacker{i}");
            var mob = TestEntityFactory.CreateMob("Target", 1000, 1000, 0);
            var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Attack };

            var result = await engine.ExecuteRoundAsync(1, [player], actions, [mob]);

            totalAttacks++;
            if (result.Messages.Any(m => m.Contains("CRIT", StringComparison.OrdinalIgnoreCase)))
            {
                critCount++;
            }
        }

        var critRate = (double)critCount / totalAttacks * 100;

        TestContext.WriteLine("Critical Hit Statistics:");
        TestContext.WriteLine($"  Total attacks: {totalAttacks}");
        TestContext.WriteLine($"  Critical hits: {critCount}");
        TestContext.WriteLine($"  Crit rate: {critRate:F1}%");
        TestContext.WriteLine("  Expected: ~15%");

        // Assert - allow �7% variance for randomness
        Assert.That(critRate, Is.InRange(8, 22),
            "Critical hit rate should be approximately 15% (�7% tolerance)");
    }

    /// <summary>
    ///     Validates that prayer (divine smite) chance is higher at low levels.
    ///     Formula: clamp(10 + (20 - level), 5, 30)
    ///     Level 1 = 29%, Level 10 = 20%, Level 20 = 10%, Level 30+ = 5%
    /// </summary>
    [Test]
    public async Task PrayerSuccess_HigherAtLowLevels()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();

        async Task<double> MeasurePrayerSuccessRate(int level, int iterations)
        {
            var successCount = 0;
            for (var i = 0; i < iterations; i++)
            {
                TestEntityFactory.ResetIdCounters();
                var player = TestEntityFactory.CreateCleric($"Faithful{i}");
                var mob = TestEntityFactory.CreateStandardMob();
                var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Pray };

                var result = await engine.ExecuteRoundAsync(level, [player], actions, [mob]);

                if (result.Messages.Any(m => m.Contains("SMITE", StringComparison.OrdinalIgnoreCase)))
                {
                    successCount++;
                }
            }

            return (double)successCount / iterations * 100;
        }

        const int sampleSize = 300;

        var level1Rate = await MeasurePrayerSuccessRate(1, sampleSize);
        var level10Rate = await MeasurePrayerSuccessRate(10, sampleSize);
        var level25Rate = await MeasurePrayerSuccessRate(25, sampleSize);

        TestContext.WriteLine("Prayer (Smite) Success Rates:");
        TestContext.WriteLine($"  Level 1:  {level1Rate:F1}% (expected ~29%)");
        TestContext.WriteLine($"  Level 10: {level10Rate:F1}% (expected ~20%)");
        TestContext.WriteLine($"  Level 25: {level25Rate:F1}% (expected ~5%)");

        // Assert - rates should decrease with level
        Assert.That(level1Rate, Is.GreaterThan(level25Rate),
            "Prayer success should be higher at low levels");
    }

    /// <summary>
    ///     Validates that mob damage scales with level but doesn't one-shot at low levels.
    /// </summary>
    [Test]
    public async Task MobDamage_ScalesReasonably()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();

        async Task<int> MeasureAverageMobDamage(int level, int mobAttackPower, int playerDefense)
        {
            var totalDamage = 0;
            const int iterations = 50;

            for (var i = 0; i < iterations; i++)
            {
                TestEntityFactory.ResetIdCounters();
                var player = TestEntityFactory.CreatePlayer($"Target{i}", defensePower: playerDefense);
                var originalHp = player.CurrentHp; // Save before combat (mutable class)
                var mob = TestEntityFactory.CreateMob("Attacker", attackPower: mobAttackPower);

                // No actions = player runs (fails), mob attacks
                var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Heal }; // Harmless action

                var result = await engine.ExecuteRoundAsync(level, [player], actions, [mob]);

                var damagedPlayer = result.Players.FirstOrDefault(p => !p.IsBot);
                if (damagedPlayer != null)
                {
                    totalDamage += originalHp - damagedPlayer.CurrentHp;
                }
            }

            return totalDamage / iterations;
        }

        var damageLevel1 = await MeasureAverageMobDamage(1, 6, 2);
        var damageLevel10 = await MeasureAverageMobDamage(10, 6, 2);
        var damageLevel20 = await MeasureAverageMobDamage(20, 6, 2);

        TestContext.WriteLine("Average Mob Damage (base ATK 6, player DEF 2):");
        TestContext.WriteLine($"  Level 1:  {damageLevel1} HP");
        TestContext.WriteLine($"  Level 10: {damageLevel10} HP");
        TestContext.WriteLine($"  Level 20: {damageLevel20} HP");

        // Assert
        Assert.That(damageLevel1, Is.LessThan(30),
            "Level 1 mobs should not one-shot a 30 HP player");
        Assert.That(damageLevel20, Is.GreaterThan(damageLevel1),
            "Damage should increase with level");
    }

    /// <summary>
    ///     Validates that defense meaningfully reduces incoming damage.
    /// </summary>
    [Test]
    public async Task Defense_MeaningfullyReducesDamage()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        const int iterations = 100;

        async Task<int> MeasureTotalDamage(int defense)
        {
            var total = 0;
            for (var i = 0; i < iterations; i++)
            {
                TestEntityFactory.ResetIdCounters();
                var player = TestEntityFactory.CreatePlayer($"Target{i}", 100, 100, defensePower: defense);
                var originalHp = player.CurrentHp; // Save before combat (mutable class)
                var mob = TestEntityFactory.CreateMob("Attacker", attackPower: 10);
                var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Heal };

                var result = await engine.ExecuteRoundAsync(1, [player], actions, [mob]);
                var damaged = result.Players.First(p => !p.IsBot);
                total += originalHp - damaged.CurrentHp;
            }

            return total;
        }

        var damageWithNoDefense = await MeasureTotalDamage(0);
        var damageWithHighDefense = await MeasureTotalDamage(8);

        TestContext.WriteLine($"Total damage over {iterations} hits:");
        TestContext.WriteLine($"  0 Defense:  {damageWithNoDefense}");
        TestContext.WriteLine($"  8 Defense:  {damageWithHighDefense}");
        TestContext.WriteLine($"  Reduction:  {damageWithNoDefense - damageWithHighDefense} ({(double)(damageWithNoDefense - damageWithHighDefense) / damageWithNoDefense:P0})");

        // Assert
        Assert.That(damageWithHighDefense, Is.LessThan(damageWithNoDefense),
            "Higher defense should reduce damage taken");
    }

    /// <summary>
    ///     Validates that healing is impactful but not overpowered.
    ///     Heal formula: MagicPower/2 + random(5-10)
    /// </summary>
    [Test]
    public async Task Healing_ImpactfulButNotOverpowered()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();
        var healAmounts = new List<int>();
        const int magicPower = 12;

        for (var i = 0; i < 100; i++)
        {
            TestEntityFactory.ResetIdCounters();
            var healer = TestEntityFactory.CreateCleric($"Healer{i}");
            healer.MagicPower = new PowerLevel(magicPower);

            var injured = TestEntityFactory.CreatePlayer($"Wounded{i}", 50, 10);
            var mob = TestEntityFactory.CreateMob("Passive", attackPower: 0);

            var actions = new Dictionary<PlayerId, PlayerAction>
            {
                [healer.Id] = PlayerAction.Heal,
                [injured.Id] = PlayerAction.Attack
            };

            var result = await engine.ExecuteRoundAsync(1, [healer, injured], actions, [mob]);
            var healed = result.Players.First(p => p.Name.StartsWith("Wounded"));
            healAmounts.Add(healed.CurrentHp - 10);
        }

        var avgHeal = healAmounts.Average();
        var minHeal = healAmounts.Min();
        var maxHeal = healAmounts.Max();

        // Expected: 12/2 + 5-10 = 6 + 5-10 = 11-16
        TestContext.WriteLine($"Heal Statistics (MagicPower={magicPower}):");
        TestContext.WriteLine($"  Average: {avgHeal:F1}");
        TestContext.WriteLine($"  Range: {minHeal} - {maxHeal}");
        TestContext.WriteLine("  Expected: 11-16 (MagicPower/2 + 5-10)");

        // Assert
        Assert.That(avgHeal, Is.InRange(10, 18),
            "Average heal should be in expected range");
        Assert.That(minHeal, Is.GreaterThanOrEqualTo(10),
            "Minimum heal should be at least MagicPower/2 + 5");
    }

    /// <summary>
    ///     Validates that run chance is reasonable and bonuses apply correctly.
    ///     Base: 40%, Low HP bonus: +25%, Outnumbered: +15%
    /// </summary>
    [Test]
    public async Task RunChance_BonusesApplyCorrectly()
    {
        // Arrange
        var engine = TestServiceFactory.CreateGameEngineWithoutBots();

        async Task<double> MeasureEscapeRate(int currentHp, int maxHp, int mobCount)
        {
            var escapes = 0;
            const int iterations = 200;

            for (var i = 0; i < iterations; i++)
            {
                TestEntityFactory.ResetIdCounters();
                var player = TestEntityFactory.CreatePlayer($"Runner{i}", maxHp, currentHp);
                var mobs = Enumerable.Range(0, mobCount)
                    .Select(m => TestEntityFactory.CreateWeakMob($"Mob{m}"))
                    .ToList();

                var actions = new Dictionary<PlayerId, PlayerAction> { [player.Id] = PlayerAction.Run };

                var result = await engine.ExecuteRoundAsync(1, [player], actions, mobs);

                if (result.Players.All(p => p.Id != player.Id))
                {
                    escapes++;
                }
            }

            return (double)escapes / iterations * 100;
        }

        var baseRate = await MeasureEscapeRate(30, 30, 1); // Full HP, 1 mob
        var lowHpRate = await MeasureEscapeRate(5, 30, 1); // Low HP bonus
        var outnumberedRate = await MeasureEscapeRate(30, 30, 3); // Outnumbered bonus
        var bothBonusesRate = await MeasureEscapeRate(5, 30, 3); // Both bonuses

        TestContext.WriteLine("Escape Rates:");
        TestContext.WriteLine($"  Base (full HP, 1 mob):       {baseRate:F1}% (expected ~40%)");
        TestContext.WriteLine($"  Low HP (5/30, 1 mob):        {lowHpRate:F1}% (expected ~65%)");
        TestContext.WriteLine($"  Outnumbered (full HP, 3 mob):{outnumberedRate:F1}% (expected ~55%)");
        TestContext.WriteLine($"  Both bonuses:                {bothBonusesRate:F1}% (expected ~80%, capped at 85%)");

        // Assert - low HP should increase escape chance
        Assert.That(lowHpRate, Is.GreaterThan(baseRate),
            "Low HP should increase escape chance");
    }
}