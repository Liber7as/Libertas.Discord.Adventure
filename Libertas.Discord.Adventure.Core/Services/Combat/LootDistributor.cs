using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Combat;

/// <summary>
///     Default implementation of <see cref="ILootDistributor" />.
///     Uses the existing Calculator to compute gold and splits among alive human players.
/// </summary>
/// <summary>
///     Default implementation of <see cref="ILootDistributor" />.
///     Uses the existing Calculator to compute gold and splits among alive human players.
/// </summary>
public class LootDistributor(ICombatCalculator calculator) : ILootDistributor
{
    private readonly ICombatCalculator _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));

    /// <summary>
    ///     Distributes loot among alive human players. Returns the total gold awarded.
    ///     Remainder from integer division is intentionally lost for fairness.
    /// </summary>
    public double DistributeLoot(CombatContext<PlayerState> context, MobState mob)
    {
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(mob);

        var gold = _calculator.CalculateLootGold(mob.AttackPower.Value, context.Level);
        var aliveHumans = context.Players.Where(p => p.IsAlive && !p.IsBot).ToList();

        if (aliveHumans.Count == 0)
        {
            return 0.0;
        }

        var perPlayer = (int)Math.Floor(gold / (double)aliveHumans.Count);
        foreach (var player in aliveHumans)
        {
            player.GoldEarned += perPlayer;
        }

        // Any remainder from integer division is intentionally not awarded to a single player
        // to keep distribution fair. Remainder is effectively lost.

        return gold;
    }
}