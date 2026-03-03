using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Core.Tests.TestUtilities;
using NUnit.Framework;

namespace Libertas.Discord.Adventure.Core.Tests;

/// <summary>
/// Tests for the skill-based player progression system.
/// Validates stat calculations, XP requirements, and skill leveling.
/// </summary>
[TestFixture]
[Category("Progression")]
public class PlayerProgressionTests
{
    private IPlayerProgressionService _progressionService = null!;

    [SetUp]
    public void SetUp()
    {
        _progressionService = TestServiceFactory.CreateProgressionService();
    }

    #region Stat Calculation Tests

    /// <summary>
    /// Verifies base stats are correct for a new player (all skills at level 1).
    /// </summary>
    [Test]
    public void CalculateStats_AllSkillsLevel1_ReturnsBaseStats()
    {
        // Arrange
        var skills = SkillLevels.Default; // All level 1

        // Act
        var stats = _progressionService.CalculateStats(skills);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(stats.MaxHp, Is.EqualTo(20), "Base HP should be 20");
            Assert.That(stats.AttackPower, Is.EqualTo(5), "Base Attack should be 5");
            Assert.That(stats.MagicPower, Is.EqualTo(5), "Base Magic should be 5");
            Assert.That(stats.SpeechPower, Is.EqualTo(5), "Base Speech should be 5");
            Assert.That(stats.DefensePower, Is.EqualTo(2), "Base Defense should be 2");
        });

        TestContext.WriteLine($"Level 1 stats: HP={stats.MaxHp}, ATK={stats.AttackPower}, MAG={stats.MagicPower}, SPE={stats.SpeechPower}, DEF={stats.DefensePower}");
    }

    /// <summary>
    /// Verifies stats scale correctly with individual skill levels.
    /// </summary>
    [Test]
    public void CalculateStats_HigherSkillLevels_ScalesCorrectly()
    {
        // Arrange - mixed skill levels
        var skills = new SkillLevels(
            AttackLevel: 10,  // +18 attack (9 levels * 2)
            MagicLevel: 5,    // +8 magic (4 levels * 2)
            SpeechLevel: 15,  // +28 speech (14 levels * 2)
            DefenseLevel: 20  // +95 HP (19 * 5), +19 defense (19 * 1)
        );

        // Act
        var stats = _progressionService.CalculateStats(skills);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(stats.MaxHp, Is.EqualTo(20 + (19 * 5)), "HP should scale with Defense level");
            Assert.That(stats.AttackPower, Is.EqualTo(5 + (9 * 2)), "Attack should scale with Attack level");
            Assert.That(stats.MagicPower, Is.EqualTo(5 + (4 * 2)), "Magic should scale with Magic level");
            Assert.That(stats.SpeechPower, Is.EqualTo(5 + (14 * 2)), "Speech should scale with Speech level");
            Assert.That(stats.DefensePower, Is.EqualTo(2 + (19 * 1)), "Defense should scale with Defense level");
        });

        TestContext.WriteLine($"Mixed level stats: HP={stats.MaxHp}, ATK={stats.AttackPower}, MAG={stats.MagicPower}, SPE={stats.SpeechPower}, DEF={stats.DefensePower}");
    }

    /// <summary>
    /// Verifies high-level characters have significantly better stats.
    /// </summary>
    [Test]
    public void CalculateStats_MaxLevel_HasSignificantStats()
    {
        // Arrange - max level in all skills (99)
        var skills = new SkillLevels(99, 99, 99, 99);

        // Act
        var stats = _progressionService.CalculateStats(skills);

        // Assert
        TestContext.WriteLine($"Max level stats: HP={stats.MaxHp}, ATK={stats.AttackPower}, MAG={stats.MagicPower}, SPE={stats.SpeechPower}, DEF={stats.DefensePower}");

        Assert.Multiple(() =>
        {
            Assert.That(stats.MaxHp, Is.GreaterThan(400), "Max level HP should be substantial");
            Assert.That(stats.AttackPower, Is.GreaterThan(150), "Max level Attack should be substantial");
            Assert.That(stats.MagicPower, Is.GreaterThan(150), "Max level Magic should be substantial");
            Assert.That(stats.SpeechPower, Is.GreaterThan(150), "Max level Speech should be substantial");
            Assert.That(stats.DefensePower, Is.GreaterThan(80), "Max level Defense should be substantial");
        });
    }

    #endregion

    #region XP Requirement Tests

    /// <summary>
    /// Verifies XP requirements for early skill levels.
    /// </summary>
    [Test]
    public void GetXpRequiredForSkillLevel_EarlyLevels_CorrectRequirements()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(_progressionService.GetXpRequiredForSkillLevel(1), Is.EqualTo(0), "Level 1 requires 0 XP");
            Assert.That(_progressionService.GetXpRequiredForSkillLevel(2), Is.EqualTo(50), "Level 2 requires 50 XP");
            Assert.That(_progressionService.GetXpRequiredForSkillLevel(3), Is.EqualTo(150), "Level 3 requires 150 XP (50 + 100)");
            Assert.That(_progressionService.GetXpRequiredForSkillLevel(4), Is.EqualTo(300), "Level 4 requires 300 XP (50 + 100 + 150)");
            Assert.That(_progressionService.GetXpRequiredForSkillLevel(5), Is.EqualTo(500), "Level 5 requires 500 XP");
        });

        TestContext.WriteLine("XP requirements for early levels:");
        for (var level = 1; level <= 10; level++)
        {
            TestContext.WriteLine($"  Level {level}: {_progressionService.GetXpRequiredForSkillLevel(level)} total XP");
        }
    }

    /// <summary>
    /// Verifies XP to next level calculation.
    /// </summary>
    [Test]
    public void GetXpToNextSkillLevel_ReturnsCorrectAmount()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(_progressionService.GetXpToNextSkillLevel(1), Is.EqualTo(50), "Level 1 ? 2 needs 50 XP");
            Assert.That(_progressionService.GetXpToNextSkillLevel(2), Is.EqualTo(100), "Level 2 ? 3 needs 100 XP");
            Assert.That(_progressionService.GetXpToNextSkillLevel(5), Is.EqualTo(250), "Level 5 ? 6 needs 250 XP");
            Assert.That(_progressionService.GetXpToNextSkillLevel(10), Is.EqualTo(500), "Level 10 ? 11 needs 500 XP");
        });
    }

    /// <summary>
    /// Verifies skill level calculation from total XP.
    /// </summary>
    [Test]
    public void GetSkillLevelForXp_ReturnsCorrectLevel()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(_progressionService.GetSkillLevelForXp(0), Is.EqualTo(1), "0 XP = Level 1");
            Assert.That(_progressionService.GetSkillLevelForXp(49), Is.EqualTo(1), "49 XP = Level 1 (not enough for 2)");
            Assert.That(_progressionService.GetSkillLevelForXp(50), Is.EqualTo(2), "50 XP = Level 2");
            Assert.That(_progressionService.GetSkillLevelForXp(149), Is.EqualTo(2), "149 XP = Level 2");
            Assert.That(_progressionService.GetSkillLevelForXp(150), Is.EqualTo(3), "150 XP = Level 3");
            Assert.That(_progressionService.GetSkillLevelForXp(500), Is.EqualTo(5), "500 XP = Level 5");
        });
    }

    #endregion

    #region Skill XP Reward Tests

    /// <summary>
    /// Verifies skill XP rewards scale with dungeon level.
    /// </summary>
    [Test]
    public void CalculateSkillXp_ScalesWithDungeonLevel()
    {
        // Arrange & Act
        var xpLevel1 = _progressionService.CalculateSkillXp(1);
        var xpLevel10 = _progressionService.CalculateSkillXp(10);
        var xpLevel50 = _progressionService.CalculateSkillXp(50);

        // Assert
        TestContext.WriteLine($"Skill XP by dungeon level: Lv1={xpLevel1}, Lv10={xpLevel10}, Lv50={xpLevel50}");

        Assert.Multiple(() =>
        {
            Assert.That(xpLevel1, Is.EqualTo(12), "Dungeon level 1: 10 base + 2*1 = 12");
            Assert.That(xpLevel10, Is.EqualTo(30), "Dungeon level 10: 10 base + 2*10 = 30");
            Assert.That(xpLevel50, Is.EqualTo(110), "Dungeon level 50: 10 base + 2*50 = 110");
        });
    }

    /// <summary>
    /// Verifies defense XP from damage taken.
    /// </summary>
    [Test]
    public void CalculateDefenseXpFromDamage_ScalesWithDamage()
    {
        // Arrange & Act
        var xpFrom10Damage = _progressionService.CalculateDefenseXpFromDamage(10);
        var xpFrom50Damage = _progressionService.CalculateDefenseXpFromDamage(50);
        var xpFrom1Damage = _progressionService.CalculateDefenseXpFromDamage(1);

        // Assert
        TestContext.WriteLine($"Defense XP from damage: 10dmg={xpFrom10Damage}, 50dmg={xpFrom50Damage}, 1dmg={xpFrom1Damage}");

        Assert.Multiple(() =>
        {
            Assert.That(xpFrom10Damage, Is.EqualTo(5), "10 damage * 0.5 = 5 XP");
            Assert.That(xpFrom50Damage, Is.EqualTo(25), "50 damage * 0.5 = 25 XP");
            Assert.That(xpFrom1Damage, Is.EqualTo(1), "1 damage * 0.5 = 1 XP (rounded up)");
        });
    }

    #endregion

    #region Combat Level Tests

    /// <summary>
    /// Verifies combat level and total level calculations.
    /// </summary>
    [Test]
    public void SkillLevels_CombatAndTotalLevel_CalculateCorrectly()
    {
        // Arrange
        var balanced = new SkillLevels(10, 10, 10, 10);
        var specialized = new SkillLevels(50, 1, 1, 1);
        var beginner = SkillLevels.Default;

        // Assert
        Assert.Multiple(() =>
        {
            // Total level = sum of all skills
            Assert.That(balanced.TotalLevel, Is.EqualTo(40), "Balanced total: 10+10+10+10=40");
            Assert.That(specialized.TotalLevel, Is.EqualTo(53), "Specialized total: 50+1+1+1=53");
            Assert.That(beginner.TotalLevel, Is.EqualTo(4), "Beginner total: 1+1+1+1=4");

            // Combat level = average (rounded)
            Assert.That(balanced.CombatLevel, Is.EqualTo(10), "Balanced combat level: 40/4=10");
            Assert.That(specialized.CombatLevel, Is.EqualTo(13), "Specialized combat level: 53/4?13");
            Assert.That(beginner.CombatLevel, Is.EqualTo(1), "Beginner combat level: 4/4=1");
        });

        TestContext.WriteLine($"Balanced: Total={balanced.TotalLevel}, Combat={balanced.CombatLevel}");
        TestContext.WriteLine($"Specialized: Total={specialized.TotalLevel}, Combat={specialized.CombatLevel}");
        TestContext.WriteLine($"Beginner: Total={beginner.TotalLevel}, Combat={beginner.CombatLevel}");
    }

    #endregion

    #region Progression Fairness Tests

    /// <summary>
    /// Verifies that leveling up provides meaningful but not overpowered stat increases.
    /// </summary>
    [Test]
    public void ProgressionCurve_MeaningfulButNotOverpowered()
    {
        // Compare level 1 vs level 10 vs level 50
        var level1Stats = _progressionService.CalculateStats(SkillLevels.Default);
        var level10Stats = _progressionService.CalculateStats(new SkillLevels(10, 10, 10, 10));
        var level50Stats = _progressionService.CalculateStats(new SkillLevels(50, 50, 50, 50));

        TestContext.WriteLine("Progression comparison:");
        TestContext.WriteLine($"  Level 1:  HP={level1Stats.MaxHp}, ATK={level1Stats.AttackPower}, MAG={level1Stats.MagicPower}");
        TestContext.WriteLine($"  Level 10: HP={level10Stats.MaxHp}, ATK={level10Stats.AttackPower}, MAG={level10Stats.MagicPower}");
        TestContext.WriteLine($"  Level 50: HP={level50Stats.MaxHp}, ATK={level50Stats.AttackPower}, MAG={level50Stats.MagicPower}");

        // Level 10 should be roughly 2-3x stronger than level 1
        var hpRatio10 = (double)level10Stats.MaxHp / level1Stats.MaxHp;
        var atkRatio10 = (double)level10Stats.AttackPower / level1Stats.AttackPower;

        TestContext.WriteLine($"  Level 10/1 ratio: HP={hpRatio10:F1}x, ATK={atkRatio10:F1}x");

        Assert.That(hpRatio10, Is.InRange(2, 5), "Level 10 should have 2-5x the HP of level 1");
        Assert.That(atkRatio10, Is.InRange(2, 6), "Level 10 should have 2-6x the Attack of level 1");
    }

    #endregion
}
