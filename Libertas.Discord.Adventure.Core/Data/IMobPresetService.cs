using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Data;

/// <summary>
///     Abstraction for accessing mob presets from the core project.
///     Implementations can fetch presets from EF, files, or other stores.
/// </summary>
public interface IMobPresetService
{
    /// <summary>
    ///     Selects a random <see cref="MobPresetDto" /> from the available presets,
    ///     optionally excluding a set of preset ids. Returns <c>null</c> when no preset
    ///     could be chosen (for example, when all presets are excluded).
    /// </summary>
    /// <param name="excludeIds">Optional set of preset ids to exclude from selection.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A randomly selected <see cref="MobPresetDto" />, or <c>null</c> if none available.</returns>
    Task<MobPresetDto?> GetRandomAsync(IReadOnlyCollection<int>? excludeIds = null, CancellationToken cancellationToken = default);
}