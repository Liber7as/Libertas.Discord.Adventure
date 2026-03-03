using Libertas.Discord.Adventure.Core.Services;

namespace Libertas.Discord.Adventure.Core.Settings;

/// <summary>
/// Settings that contain localized message templates used by <see cref="IActionLocalizationService"/>.
/// Each list holds possible messages that implementations may choose from at runtime.
/// </summary>
public class LocalizationSettings
{
    /// <summary>Possible templates for attack messages.</summary>
    public List<string> AttackMessages { get; set; } = new();

    /// <summary>Possible templates for messages when an attack hits nothing.</summary>
    public List<string> AttackNothingMessages { get; set; } = new();

    /// <summary>Possible templates for critical attack messages.</summary>
    public List<string> AttackCritMessages { get; set; } = new();

    /// <summary>Possible templates for magic action messages.</summary>
    public List<string> MagicMessages { get; set; } = new();

    /// <summary>Possible templates when magic has no target.</summary>
    public List<string> MagicNoTargetMessages { get; set; } = new();

    /// <summary>Possible templates for magic critical messages.</summary>
    public List<string> MagicCritMessages { get; set; } = new();

    /// <summary>Possible templates when talk has no target.</summary>
    public List<string> TalkNoTargetMessages { get; set; } = new();

    /// <summary>Possible templates for successful talk messages.</summary>
    public List<string> TalkSuccessMessages { get; set; } = new();

    /// <summary>Possible templates for failed talk messages.</summary>
    public List<string> TalkFailMessages { get; set; } = new();

    /// <summary>Possible templates when pray has no target.</summary>
    public List<string> PrayNoTargetMessages { get; set; } = new();

    /// <summary>Possible templates for successful pray messages.</summary>
    public List<string> PraySuccessMessages { get; set; } = new();

    /// <summary>Possible templates for failed pray messages.</summary>
    public List<string> PrayFailMessages { get; set; } = new();

    /// <summary>Possible templates for pray heal messages.</summary>
    public List<string> PrayHealMessages { get; set; } = new();

    /// <summary>Possible templates for successful run messages.</summary>
    public List<string> RunSuccessMessages { get; set; } = new();

    /// <summary>Possible templates for failed run messages.</summary>
    public List<string> RunFailMessages { get; set; } = new();

    /// <summary>Possible templates for run-fail-when-outnumbered messages.</summary>
    public List<string> RunFailOutnumberedMessages { get; set; } = new();

    /// <summary>Possible templates for mob attack messages.</summary>
    public List<string> MobAttackMessages { get; set; } = new();

    /// <summary>Possible templates for damage reduced messages.</summary>
    public List<string> DamageReducedMessages { get; set; } = new();

    /// <summary>Possible templates for mob killed messages.</summary>
    public List<string> MobKilledMessages { get; set; } = new();

    /// <summary>Possible templates for player killed messages.</summary>
    public List<string> PlayerKilledMessages { get; set; } = new();

    /// <summary>Possible templates for loot messages.</summary>
    public List<string> LootMessages { get; set; } = new();

    /// <summary>Possible templates for loot remainder messages.</summary>
    public List<string> LootRemainderMessages { get; set; } = new();

    /// <summary>Possible templates for a party loot summary message.</summary>
    public List<string> PartyLootSummaryMessages { get; set; } = new();

    /// <summary>Possible templates for heal messages.</summary>
    public List<string> HealMessages { get; set; } = new();

    /// <summary>Possible templates for heal-no-target messages.</summary>
    public List<string> HealNoTargetMessages { get; set; } = new();

    /// <summary>Possible location/teaser strings used for level introductions.</summary>
    public List<string> Locations { get; set; } = new();
}
