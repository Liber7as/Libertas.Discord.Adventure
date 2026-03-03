using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Combat;

namespace Libertas.Discord.Adventure.Core.Services.Actions;

/// <summary>
/// Handles the Talk action - attempt to defeat a mob through diplomacy.
/// Success chance is based on SpeechPower vs mob's AttackPower.
/// </summary>
public class TalkActionHandler(
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
    public override PlayerAction Action => PlayerAction.Talk;

    /// <inheritdoc />
    public override void Execute(CombatContext<PlayerState> context)
    {
        var target = Calculator.SelectRandomTarget(context.Mobs);
        if (target == null)
        {
            context.Messages.Add(Localization.GetTalkNoTargetMessage(context.Actor.Name));
            return;
        }

        // Award Speech XP regardless of success
        _xpDistributor.AwardSkillXp(context.Actor, SkillType.Speech, context.Level);

        // Calculate and roll success
        var successChance = Calculator.CalculateTalkSuccessChance(
            context.Actor.SpeechPower.Value,
            target.AttackPower.Value,
            context.Level);

        if (Calculator.RollChance(successChance))
        {
            context.Messages.Add(Localization.GetTalkSuccessMessage(context.Actor.Name, target.Name));

            if (target.CurrentHp > 0)
            {
                // Use damage applier to set HP to zero consistently
                _damageApplier.ApplyDamageToMob(target, target.CurrentHp, context);
                AwardKill(context, target);
                _lootDistributor.DistributeLoot(context, target);
            }
        }
        else
        {
            context.Messages.Add(Localization.GetTalkFailMessage(context.Actor.Name, target.Name));
        }
    }
}
