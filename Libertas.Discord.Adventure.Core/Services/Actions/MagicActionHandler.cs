using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Combat;

namespace Libertas.Discord.Adventure.Core.Services.Actions;

/// <summary>
/// Handles the Magic action - magical damage with critical hit chance.
/// </summary>
public class MagicActionHandler(
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
    public override PlayerAction Action => PlayerAction.Magic;

    /// <inheritdoc />
    public override void Execute(CombatContext<PlayerState> context)
    {
        var target = Calculator.SelectRandomTarget(context.Mobs);
        if (target == null)
        {
            return;
        }

        // Calculate damage
        var (damage, isCrit) = Calculator.CalculateMagicDamage(context.Actor.MagicPower.Value);

        // Award XP for using the skill
        _xpDistributor.AwardSkillXp(context.Actor, SkillType.Magic, context.Level);

        // Apply damage via DamageApplier
        var oldHp = target.CurrentHp;
        _ = _damageApplier.ApplyDamageToMob(target, damage, context);
        var newHp = target.CurrentHp;

        // Add message
        var message = isCrit
            ? Localization.GetMagicCritMessage(context.Actor.Name, target.Name, damage, oldHp, newHp)
            : Localization.GetMagicMessage(context.Actor.Name, target.Name, damage, oldHp, newHp);
        context.Messages.Add(message);

        // Handle kill
        if (newHp == 0 && oldHp > 0)
        {
            AwardKill(context, target);
            _lootDistributor.DistributeLoot(context, target);
        }
    }
}
