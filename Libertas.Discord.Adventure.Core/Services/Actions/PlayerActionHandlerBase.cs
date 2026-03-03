using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Actions;

/// <summary>
///     Base class for player action handlers providing common functionality.
/// </summary>
public abstract class PlayerActionHandlerBase(
    ICombatCalculator calculator,
    IActionLocalizationService localization,
    IPlayerProgressionService progression) : IPlayerActionHandler
{
    /// <summary>
    ///     Calculator used for combat math (damage, loot, etc.).
    /// </summary>
    protected readonly ICombatCalculator Calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));

    /// <summary>
    ///     Localization service used to produce user-facing messages.
    /// </summary>
    protected readonly IActionLocalizationService Localization = localization ?? throw new ArgumentNullException(nameof(localization));

    /// <summary>
    ///     Progression service used to award XP and calculate level-related values.
    /// </summary>
    protected readonly IPlayerProgressionService Progression = progression ?? throw new ArgumentNullException(nameof(progression));

    /// <inheritdoc />
    public abstract PlayerAction Action { get; }

    /// <inheritdoc />
    public abstract void Execute(CombatContext<PlayerState> context);

    /// <summary>
    ///     Awards XP for using a skill. Bots do not receive XP.
    /// </summary>
    protected void AwardSkillXp(PlayerState player, SkillType skill, int dungeonLevel)
    {
        if (player.IsBot)
        {
            return;
        }

        var xp = Progression.CalculateSkillXp(dungeonLevel);

        switch (skill)
        {
            case SkillType.Attack:
                player.AttackXpEarned += xp;
                break;
            case SkillType.Magic:
                player.MagicXpEarned += xp;
                break;
            case SkillType.Speech:
                player.SpeechXpEarned += xp;
                break;
        }
    }

    /// <summary>
    ///     Awards a kill to the actor and distributes loot.
    /// </summary>
    protected void AwardKill(CombatContext<PlayerState> context, MobState mob)
    {
        context.Messages.Add(Localization.GetMobKilledMessage(context.Actor.Name, mob.Name));

        // Credit the kill to the actor (bots don't get credit)
        if (!context.Actor.IsBot)
        {
            context.Actor.MobsKilled++;
        }

        AwardLoot(context, mob);
    }

    /// <summary>
    ///     Distributes loot from a defeated mob to alive human players.
    /// </summary>
    private void AwardLoot(CombatContext<PlayerState> context, MobState mob)
    {
        var gold = Calculator.CalculateLootGold(mob.AttackPower.Value, context.Level);
        context.Messages.Add(Localization.GetLootMessage(mob.Name, gold));

        var aliveHumans = context.Players.Where(p => p.IsAlive && !p.IsBot).ToList();
        if (aliveHumans.Count == 0)
        {
            return;
        }

        var perPlayer = gold / aliveHumans.Count;
        foreach (var player in aliveHumans)
        {
            player.GoldEarned += perPlayer;
        }

        context.Messages.Add(Localization.GetPartyLootSummaryMessage(gold, aliveHumans.Count));

        var remainder = gold % aliveHumans.Count;
        if (remainder > 0)
        {
            context.Messages.Add(Localization.GetLootRemainderMessage(remainder));
        }
    }

    // SkillType enum moved to shared model `Libertas.Discord.Adventure.Core.GameModels.SkillType`
}