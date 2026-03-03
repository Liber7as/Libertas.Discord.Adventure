using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Discord.Services;

/// <summary>
/// Factory service responsible for creating <see cref="MobState"/> instances from presets
/// and scaling them for a given dungeon level.
/// </summary>
public interface IMobFactoryService
{
    /// <summary>
    ///     Create a mob from an explicit preset and scale it for the provided level.
    /// </summary>
    Task<MobState> CreateFromPresetAsync(MobPresetDto preset, int level);

    /// <summary>
    ///     Choose a preset (optionally excluding some ids) and create a mob scaled for the provided level.
    ///     Returns null when no preset could be chosen.
    /// </summary>
    Task<MobState?> CreateRandomMobForLevelAsync(int level, IReadOnlyCollection<int>? excludePresetIds = null, CancellationToken cancellationToken = default);
}