using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services.Combat;

namespace Libertas.Discord.Adventure.Core.Services.Actions;

/// <summary>
///     Handles the Run action - attempt to flee from combat.
///     Success chance is modified by HP status and being outnumbered.
/// </summary>
public class RunActionHandler(
    ICombatCalculator calculator,
    IActionLocalizationService localization,
    IPlayerProgressionService progression,
    ILootDistributor lootDistributor) : PlayerActionHandlerBase(calculator, localization, progression)
{
    private readonly ILootDistributor _lootDistributor = lootDistributor ?? throw new ArgumentNullException(nameof(lootDistributor));

    /// <inheritdoc />
    public override PlayerAction Action => PlayerAction.Run;

    /// <inheritdoc />
    public override void Execute(CombatContext<PlayerState> context)
    {
        // Run does not award XP - it's an escape action

        var runChance = Calculator.CalculateRunChance(
            context.Actor.CurrentHp,
            context.Actor.MaxHp,
            context.Players.Count,
            context.Mobs.Count);

        if (Calculator.RollChance(runChance))
        {
            // Successfully escaped
            context.Players.RemoveAll(p => p.Id == context.Actor.Id);
            context.Messages.Add(Localization.GetRunSuccessMessage(context.Actor.Name));
        }
        else
        {
            // Failed to escape
            context.Messages.Add(Localization.GetRunFailMessage(context.Actor.Name));

            if (context.Mobs.Count > context.Players.Count)
            {
                context.Messages.Add(Localization.GetRunFailOutnumberedMessage(context.Actor.Name));
            }
        }
    }
}