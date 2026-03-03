namespace Libertas.Discord.Adventure.Core.GameModels;

/// <summary>
///     Actions a player can choose during a round.
/// </summary>
public enum PlayerAction
{
    /// <summary>
    ///     Perform a physical attack against a mob.
    /// </summary>
    Attack,

    /// <summary>
    ///     Cast a magic spell (damage/heal) against a mob or ally.
    /// </summary>
    Magic,

    /// <summary>
    ///     Use speech/diplomacy to attempt to defeat or influence a mob.
    /// </summary>
    Talk,

    /// <summary>
    ///     Attempt a risky action with a chance for big effect (pray).
    /// </summary>
    Pray,

    /// <summary>
    ///     Heal an ally.
    /// </summary>
    Heal,

    /// <summary>
    ///     Attempt to flee from combat.
    /// </summary>
    Run
}