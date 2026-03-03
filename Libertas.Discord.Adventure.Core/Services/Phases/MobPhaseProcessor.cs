using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Phases;

/// <summary>
///     Handles the mob phase of the round, where mobs attack players.
/// </summary>
public class MobPhaseProcessor(IActionResolutionService actionResolutionService)
{
    private readonly IActionResolutionService _actionResolutionService = actionResolutionService;

    /// <summary>
    ///     Executes all mob attacks. Dead mobs are skipped.
    ///     Each alive mob attacks a player (targeting logic is in the mob action handler).
    /// </summary>
    public void Execute(
        List<PlayerState> playerList,
        List<MobState> mobList,
        int level,
        List<string> messages)
    {
        var aliveMobs = mobList.Where(m => m.IsAlive).ToList();

        foreach (var mob in aliveMobs)
        {
            var mobContext = new CombatContext<MobState>(level, mob, playerList, mobList, messages);
            _actionResolutionService.HandleMobAction(mobContext);
        }
    }
}