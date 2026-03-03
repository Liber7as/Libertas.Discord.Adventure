using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Combat;

namespace Libertas.Discord.Adventure.Core.Services.Actions;

/// <summary>
///     Handles the Heal action - restore HP to an injured ally.
///     Cannot heal self; targets a random injured ally.
/// </summary>
public class HealActionHandler(
    ICombatCalculator calculator,
    IActionLocalizationService localization,
    IPlayerProgressionService progression,
    IXpDistributor xpDistributor) : PlayerActionHandlerBase(calculator, localization, progression)
{
    private readonly IXpDistributor _xpDistributor = xpDistributor ?? throw new ArgumentNullException(nameof(xpDistributor));

    /// <inheritdoc />
    public override PlayerAction Action => PlayerAction.Heal;

    /// <inheritdoc />
    public override void Execute(CombatContext<PlayerState> context)
    {
        // Find injured allies (not self)
        var injured = context.Players
            .Where(p => p.IsAlive && p.CurrentHp < p.MaxHp && p.Id != context.Actor.Id)
            .ToList();

        if (injured.Count == 0)
        {
            context.Messages.Add(Localization.GetHealNoTargetMessage(context.Actor.Name));
            return;
        }

        var target = Calculator.SelectRandomTarget(injured);
        if (target == null || !target.IsAlive || target.CurrentHp >= target.MaxHp)
        {
            context.Messages.Add(Localization.GetHealNoTargetMessage(context.Actor.Name));
            return;
        }

        // Award Magic XP
        _xpDistributor.AwardSkillXp(context.Actor, SkillType.Magic, context.Level);

        // Calculate and apply healing
        var healAmount = Calculator.CalculateHealAmount(context.Actor.MagicPower.Value);
        var oldHp = target.CurrentHp;
        var newHp = Math.Min(target.MaxHp, oldHp + healAmount);

        context.Messages.Add(Localization.GetHealMessage(context.Actor.Name, target.Name, healAmount, oldHp, newHp));

        target.CurrentHp = newHp;
    }
}