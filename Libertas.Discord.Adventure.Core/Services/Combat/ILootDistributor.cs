using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Combat;

/// <summary>
///     Responsible for distributing loot (gold) from defeated mobs to alive human players.
/// </summary>
public interface ILootDistributor
{
    /// <summary>
    ///     Distributes loot for the given mob among alive human players in the context.
    ///     Returns the total gold awarded.
    /// </summary>
    double DistributeLoot(CombatContext<PlayerState> context, MobState mob);
}