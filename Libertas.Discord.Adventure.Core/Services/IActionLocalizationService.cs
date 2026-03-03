namespace Libertas.Discord.Adventure.Core.Services;

/// <summary>
/// Provides localized message text for player and mob actions.
/// Implementations should return human-readable strings used in Discord messages.
/// </summary>
public interface IActionLocalizationService
{
    /// <summary>
    /// Message for a normal attack.
    /// </summary>
    string GetAttackMessage(string actor, string target, int damage, int oldHp, int newHp);

    /// <summary>
    /// Message for a critical attack.
    /// </summary>
    string GetAttackCritMessage(string actor, string target, int damage, int oldHp, int newHp);

    /// <summary>
    /// Message when an attack does no damage.
    /// </summary>
    string GetAttackNothingMessage(string actor);

    /// <summary>
    /// Message for a normal magic action.
    /// </summary>
    string GetMagicMessage(string actor, string target, int damage, int oldHp, int newHp);

    /// <summary>
    /// Message for a magic critical hit.
    /// </summary>
    string GetMagicCritMessage(string actor, string target, int damage, int oldHp, int newHp);

    /// <summary>
    /// Message when magic has no valid target.
    /// </summary>
    string GetMagicNoTargetMessage(string actor);

    /// <summary>
    /// Message when a talk action has no target.
    /// </summary>
    string GetTalkNoTargetMessage(string actor);

    /// <summary>
    /// Message for a successful talk action.
    /// </summary>
    string GetTalkSuccessMessage(string actor, string target);

    /// <summary>
    /// Message for a failed talk action.
    /// </summary>
    string GetTalkFailMessage(string actor, string target);

    /// <summary>
    /// Message when pray action has no valid target.
    /// </summary>
    string GetPrayNoTargetMessage(string actor);

    /// <summary>
    /// Message for a successful pray action.
    /// </summary>
    string GetPraySuccessMessage(string actor, string target);

    /// <summary>
    /// Message for a failed pray action.
    /// </summary>
    string GetPrayFailMessage(string actor);

    /// <summary>
    /// Message for a pray that heals the actor.
    /// </summary>
    string GetPrayHealMessage(string actor, int heal, int oldHp, int newHp);

    /// <summary>
    /// Message for a successful run (escape).
    /// </summary>
    string GetRunSuccessMessage(string actor);

    /// <summary>
    /// Message for a failed run attempt.
    /// </summary>
    string GetRunFailMessage(string actor);

    /// <summary>
    /// Message for a failed run due to being outnumbered.
    /// </summary>
    string GetRunFailOutnumberedMessage(string actor);

    /// <summary>
    /// Message describing a mob attacking a player.
    /// </summary>
    string GetMobAttackMessage(string actor, string target, int damage, int oldHp, int newHp);

    /// <summary>
    /// Message indicating damage was partially reduced/blocked.
    /// </summary>
    string GetDamageReducedMessage(string target, int reduction);

    /// <summary>
    /// Message for when a mob is killed by a player.
    /// </summary>
    string GetMobKilledMessage(string killer, string mob);

    /// <summary>
    /// Message for when a player is killed by a mob.
    /// </summary>
    string GetPlayerKilledMessage(string mob, string player);

    /// <summary>
    /// Message announcing loot dropped by a mob.
    /// </summary>
    string GetLootMessage(string mob, int gold);

    /// <summary>
    /// Message indicating remainder gold that was not evenly split.
    /// </summary>
    string GetLootRemainderMessage(int remainder);

    /// <summary>
    /// Short summary of party loot distribution (total gold and alive count).
    /// </summary>
    string GetPartyLootSummaryMessage(int totalGold, int aliveCount);

    /// <summary>
    /// Message for a heal action.
    /// </summary>
    string GetHealMessage(string healer, string target, int amount, int oldHp, int newHp);

    /// <summary>
    /// Message when a heal action has no valid target.
    /// </summary>
    string GetHealNoTargetMessage(string healer);

    /// <summary>
    /// Teaser text shown at the start of a level.
    /// </summary>
    string GetTeaserMessage();
}
