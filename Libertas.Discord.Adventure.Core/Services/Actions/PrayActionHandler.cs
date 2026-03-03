using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Combat;

namespace Libertas.Discord.Adventure.Core.Services.Actions;

/// <summary>
///     Handles the Pray action - chance to instantly kill a mob (Divine Smite),
///     or heal self if the prayer fails.
/// </summary>
public class PrayActionHandler(
    ICombatCalculator calculator,
    IActionLocalizationService localization,
    IPlayerProgressionService progression,
    IXpDistributor xpDistributor,
    ILootDistributor lootDistributor,
    IDamageApplier damageApplier) : PlayerActionHandlerBase(calculator, localization, progression)
{
    private readonly IDamageApplier _damageApplier = damageApplier ?? throw new ArgumentNullException(nameof(damageApplier));
    private readonly ILootDistributor _lootDistributor = lootDistributor ?? throw new ArgumentNullException(nameof(lootDistributor));
    private readonly IXpDistributor _xpDistributor = xpDistributor ?? throw new ArgumentNullException(nameof(xpDistributor));

    /// <inheritdoc />
    public override PlayerAction Action => PlayerAction.Pray;

    /// <inheritdoc />
    public override void Execute(CombatContext<PlayerState> context)
    {
        var target = Calculator.SelectRandomTarget(context.Mobs);
        if (target == null)
        {
            context.Messages.Add(Localization.GetPrayNoTargetMessage(context.Actor.Name));
            return;
        }

        // Award Magic XP (prayer uses divine/magical energy)
        _xpDistributor.AwardSkillXp(context.Actor, SkillType.Magic, context.Level);

        // Calculate and roll for divine smite
        var smiteChance = Calculator.CalculatePraySmiteChance(context.Level);

        if (Calculator.RollChance(smiteChance))
        {
            // Divine Smite succeeds - instant kill
            context.Messages.Add(Localization.GetPraySuccessMessage(context.Actor.Name, target.Name));

            if (target.CurrentHp > 0)
            {
                _damageApplier.ApplyDamageToMob(target, target.CurrentHp, context);
                AwardKill(context, target);
                _lootDistributor.DistributeLoot(context, target);
            }
        }
        else
        {
            // Prayer fails - heal self instead
            var healAmount = Calculator.CalculatePrayHealAmount(context.Actor.MaxHp);
            var oldHp = context.Actor.CurrentHp;
            var newHp = Math.Min(context.Actor.MaxHp, oldHp + healAmount);

            context.Messages.Add(Localization.GetPrayFailMessage(context.Actor.Name));
            context.Messages.Add(Localization.GetPrayHealMessage(context.Actor.Name, healAmount, oldHp, newHp));

            context.Actor.CurrentHp = newHp;
        }
    }
}