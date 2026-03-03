using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Actions;

/// <summary>
///     Handles mob attack actions - targets players with preference for lowest HP.
/// </summary>
public class MobActionHandler(
    ICombatCalculator calculator,
    IActionLocalizationService localization,
    IPlayerProgressionService progression) : IMobActionHandler
{
    private readonly ICombatCalculator _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
    private readonly IActionLocalizationService _localization = localization ?? throw new ArgumentNullException(nameof(localization));
    private readonly IPlayerProgressionService _progression = progression ?? throw new ArgumentNullException(nameof(progression));

    /// <inheritdoc />
    public void Execute(CombatContext<MobState> context)
    {
        // Select target (prefers lowest HP)
        var target = _calculator.SelectMobTarget(context.Players);
        if (target == null || !target.IsAlive)
        {
            return;
        }

        // Calculate damage
        var rawDamage = _calculator.CalculateMobDamage(context.Actor.AttackPower.Value, context.Level);
        var (reducedDamage, blocked) = _calculator.ApplyDefenseReduction(rawDamage, target.DefensePower.Value);

        // Apply damage using DamageApplier logic inline for now (future: inject IDamageApplier)
        var oldHp = target.CurrentHp;
        var newHp = Math.Max(0, oldHp - reducedDamage);
        target.CurrentHp = newHp;

        // Add attack message
        context.Messages.Add(_localization.GetMobAttackMessage(
            context.Actor.Name, target.Name, reducedDamage, oldHp, newHp));

        // Add defense message if damage was blocked
        if (blocked > 0)
        {
            context.Messages.Add(_localization.GetDamageReducedMessage(target.Name, blocked));
        }

        // Award Defense XP to the target (if not a bot)
        if (!target.IsBot)
        {
            target.DefenseXpEarned += _progression.CalculateDefenseXpFromDamage(reducedDamage);
        }

        // Check for death
        if (newHp == 0 && oldHp > 0)
        {
            context.Messages.Add(_localization.GetPlayerKilledMessage(context.Actor.Name, target.Name));
        }
    }
}