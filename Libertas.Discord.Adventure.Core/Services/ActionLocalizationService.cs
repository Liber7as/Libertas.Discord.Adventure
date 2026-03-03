using Libertas.Discord.Adventure.Core.Settings;
using Microsoft.Extensions.Options;

namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
///     Provides localized/narrative text for combat actions using random templates from settings.
/// </summary>
/// <remarks>
///     Centralizes all narrative and feedback text for player and mob actions.
/// </remarks>
public class ActionLocalizationService(IRandomNumberGenerator rng, IOptions<LocalizationSettings> options) : IActionLocalizationService
{
    private readonly LocalizationSettings _settings = options.Value;

    /// <summary>
    ///     Gets a teaser message for the start of an adventure.
    /// </summary>
    public string GetTeaserMessage()
    {
        var t = rng.GetRandom(_settings.Locations) ?? string.Empty;
        return $"On your adventure, {t}{Environment.NewLine}Up ahead lies...";
    }

    /// <summary>
    ///     Gets a localized attack message.
    /// </summary>
    public string GetAttackMessage(string actor, string target, int damage, int oldHp, int newHp)
    {
        var t = rng.GetRandom(_settings.AttackMessages) ?? string.Empty;
        return string.Format(t, actor, target, damage, oldHp, newHp);
    }

    /// <summary>
    ///     Gets a localized critical attack message.
    /// </summary>
    public string GetAttackCritMessage(string actor, string target, int damage, int oldHp, int newHp)
    {
        var t = rng.GetRandom(_settings.AttackCritMessages) ?? string.Empty;
        return string.Format(t, actor, target, damage, oldHp, newHp);
    }

    /// <summary>
    ///     Gets a localized message for a failed attack.
    /// </summary>
    public string GetAttackNothingMessage(string actor)
    {
        var t = rng.GetRandom(_settings.AttackNothingMessages) ?? string.Empty;
        return string.Format(t, actor);
    }

    /// <summary>
    ///     Gets a localized magic attack message.
    /// </summary>
    public string GetMagicMessage(string actor, string target, int damage, int oldHp, int newHp)
    {
        var t = rng.GetRandom(_settings.MagicMessages) ?? string.Empty;
        return string.Format(t, actor, target, damage, oldHp, newHp);
    }

    /// <summary>
    ///     Gets a localized critical magic attack message.
    /// </summary>
    public string GetMagicCritMessage(string actor, string target, int damage, int oldHp, int newHp)
    {
        var t = rng.GetRandom(_settings.MagicCritMessages) ?? string.Empty;
        return string.Format(t, actor, target, damage, oldHp, newHp);
    }

    /// <summary>
    ///     Gets a localized message for a failed magic attack.
    /// </summary>
    public string GetMagicNoTargetMessage(string actor)
    {
        var t = rng.GetRandom(_settings.MagicNoTargetMessages) ?? string.Empty;
        return string.Format(t, actor);
    }

    /// <summary>
    ///     Gets a localized message for a failed talk action.
    /// </summary>
    public string GetTalkNoTargetMessage(string actor)
    {
        var t = rng.GetRandom(_settings.TalkNoTargetMessages) ?? string.Empty;
        return string.Format(t, actor);
    }

    /// <summary>
    ///     Gets a localized message for a successful talk action.
    /// </summary>
    public string GetTalkSuccessMessage(string actor, string target)
    {
        var t = rng.GetRandom(_settings.TalkSuccessMessages) ?? string.Empty;
        return string.Format(t, actor, target);
    }

    /// <summary>
    ///     Gets a localized message for a failed talk action.
    /// </summary>
    public string GetTalkFailMessage(string actor, string target)
    {
        var t = rng.GetRandom(_settings.TalkFailMessages) ?? string.Empty;
        return string.Format(t, actor, target);
    }

    /// <summary>
    ///     Gets a localized message for a failed pray action.
    /// </summary>
    public string GetPrayNoTargetMessage(string actor)
    {
        var t = rng.GetRandom(_settings.PrayNoTargetMessages) ?? string.Empty;
        return string.Format(t, actor);
    }

    /// <summary>
    ///     Gets a localized message for a successful pray action.
    /// </summary>
    public string GetPraySuccessMessage(string actor, string target)
    {
        var t = rng.GetRandom(_settings.PraySuccessMessages) ?? string.Empty;
        return string.Format(t, actor, target);
    }

    /// <summary>
    ///     Gets a localized message for a failed pray action.
    /// </summary>
    public string GetPrayFailMessage(string actor)
    {
        var t = rng.GetRandom(_settings.PrayFailMessages) ?? string.Empty;
        return string.Format(t, actor);
    }

    /// <summary>
    ///     Gets a localized message for a successful pray heal.
    /// </summary>
    public string GetPrayHealMessage(string actor, int heal, int oldHp, int newHp)
    {
        var t = rng.GetRandom(_settings.PrayHealMessages) ?? string.Empty;
        return string.Format(t, actor, heal, oldHp, newHp);
    }

    /// <summary>
    ///     Gets a localized message for a successful run action.
    /// </summary>
    public string GetRunSuccessMessage(string actor)
    {
        var t = rng.GetRandom(_settings.RunSuccessMessages) ?? string.Empty;
        return string.Format(t, actor);
    }

    /// <summary>
    ///     Gets a localized message for a failed run action.
    /// </summary>
    public string GetRunFailMessage(string actor)
    {
        var t = rng.GetRandom(_settings.RunFailMessages) ?? string.Empty;
        return string.Format(t, actor);
    }

    /// <summary>
    ///     Gets a localized message for a failed run action due to being outnumbered.
    /// </summary>
    public string GetRunFailOutnumberedMessage(string actor)
    {
        var t = rng.GetRandom(_settings.RunFailOutnumberedMessages) ?? string.Empty;
        return string.Format(t, actor);
    }

    /// <summary>
    ///     Gets a localized message for a mob attack.
    /// </summary>
    public string GetMobAttackMessage(string actor, string target, int damage, int oldHp, int newHp)
    {
        var t = rng.GetRandom(_settings.MobAttackMessages) ?? string.Empty;
        return string.Format(t, actor, target, damage, oldHp, newHp);
    }

    /// <summary>
    ///     Gets a localized message for damage reduction.
    /// </summary>
    public string GetDamageReducedMessage(string target, int reduction)
    {
        var t = rng.GetRandom(_settings.DamageReducedMessages) ?? string.Empty;
        return string.Format(t, target, reduction);
    }

    /// <summary>
    ///     Gets a localized message for a mob being killed.
    /// </summary>
    public string GetMobKilledMessage(string killer, string mob)
    {
        var t = rng.GetRandom(_settings.MobKilledMessages) ?? string.Empty;
        return string.Format(t, killer, mob);
    }

    /// <summary>
    ///     Gets a localized message for loot earned from a mob.
    /// </summary>
    public string GetLootMessage(string mob, int gold)
    {
        var t = rng.GetRandom(_settings.LootMessages) ?? string.Empty;
        return string.Format(t, mob, gold);
    }

    /// <summary>
    ///     Gets a localized message for loot remainder distribution.
    /// </summary>
    public string GetLootRemainderMessage(int remainder)
    {
        var t = rng.GetRandom(_settings.LootRemainderMessages) ?? string.Empty;
        return string.Format(t, remainder);
    }

    /// <summary>
    ///     Gets a localized message for party loot summary.
    /// </summary>
    public string GetPartyLootSummaryMessage(int totalGold, int aliveCount)
    {
        var t = rng.GetRandom(_settings.PartyLootSummaryMessages) ?? string.Empty;
        return string.Format(t, totalGold, aliveCount);
    }

    /// <summary>
    ///     Gets a localized message for a player being killed by a mob.
    /// </summary>
    public string GetPlayerKilledMessage(string mob, string player)
    {
        var t = rng.GetRandom(_settings.PlayerKilledMessages) ?? string.Empty;
        return string.Format(t, mob, player);
    }

    /// <summary>
    ///     Gets a localized message for a successful heal action.
    /// </summary>
    public string GetHealMessage(string healer, string target, int amount, int oldHp, int newHp)
    {
        var t = rng.GetRandom(_settings.HealMessages) ?? string.Empty;
        return string.Format(t, healer, target, amount, oldHp, newHp);
    }

    /// <summary>
    ///     Gets a localized message for a failed heal action.
    /// </summary>
    public string GetHealNoTargetMessage(string healer)
    {
        var t = rng.GetRandom(_settings.HealNoTargetMessages) ?? string.Empty;
        return string.Format(t, healer);
    }
}