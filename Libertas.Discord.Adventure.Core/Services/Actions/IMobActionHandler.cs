using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Services.Actions;

/// <summary>
/// Handles mob attack execution.
/// </summary>
public interface IMobActionHandler
{
    /// <summary>
    /// Executes the mob's attack action.
    /// </summary>
    /// <param name="context">The current combat state with the mob as actor.</param>
    void Execute(CombatContext<MobState> context);
}