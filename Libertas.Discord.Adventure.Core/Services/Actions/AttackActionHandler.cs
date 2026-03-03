using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Combat;

namespace Libertas.Discord.Adventure.Core.Services.Actions;

/// <summary>
/// Handles the Attack action - physical damage with critical hit chance.
/// </summary>
public class AttackActionHandler(
    ICombatCalculator calculator,
    IActionLocalizationService localization,
    IPlayerProgressionService progression,
    IXpDistributor xpDistributor,
    ILootDistributor lootDistributor,
    IDamageApplier damageApplier) : PlayerActionHandlerBase(calculator, localization, progression)
{
    private readonly IXpDistributor _xpDistributor = xpDistributor ?? throw new ArgumentNullException(nameof(xpDistributor));
    private readonly ILootDistributor _lootDistributor = lootDistributor ?? throw new ArgumentNullException(nameof(lootDistributor));
    private readonly IDamageApplier _damageApplier = damageApplier ?? throw new ArgumentNullException(nameof(damageApplier));

    /// <inheritdoc />
    public override PlayerAction Action => PlayerAction.Attack;

    /// <inheritdoc />
    public override void Execute(CombatContext<PlayerState> context)
    {
        var target = Calculator.SelectRandomTarget(context.Mobs);
        if (target == null)
        {
            return;
        }

        // Calculate damage
        var (damage, isCrit) = Calculator.CalculateAttackDamage(context.Actor.AttackPower.Value);

        // Award XP for using the skill
        _xpDistributor.AwardSkillXp(context.Actor, SkillType.Attack, context.Level);

        // Apply damage via DamageApplier
        var oldHp = target.CurrentHp;
        _ = _damageApplier.ApplyDamageToMob(target, damage, context);
        var newHp = target.CurrentHp;

        // Add message
        var message = isCrit
            ? Localization.GetAttackCritMessage(context.Actor.Name, target.Name, damage, oldHp, newHp)
            : Localization.GetAttackMessage(context.Actor.Name, target.Name, damage, oldHp, newHp);
        context.Messages.Add(message);

        // Handle kill
        if (newHp == 0 && oldHp > 0)
        {
            AwardKill(context, target);
            // Distribute loot
            _ = _lootDistributor.DistributeLoot(context, target);
            // add a summary message (the AwardLoot method previously added messages)
            // The AwardLoot call inside AwardKill previously added messages; keep behavior consistent
        }
    }
}
