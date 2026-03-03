using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Libertas.Discord.Adventure.Core.Data;
using Libertas.Discord.Adventure.Discord.Data;
using Microsoft.Extensions.Logging;

namespace Libertas.Discord.Adventure.Discord.Services;

/// <summary>
/// Service for creating <see cref="MobState"/> instances from presets and level scaling.
/// Encapsulates logic for randomizing mob stats and selecting unused presets.
/// </summary>
/// <remarks>
/// Used to generate mobs for each round, ensuring variety and appropriate difficulty scaling.
/// </remarks>
public class MobFactoryService(Libertas.Discord.Adventure.Core.Data.IMobPresetService presetService, IRandomNumberGenerator rng, ILogger<MobFactoryService> logger) : IMobFactoryService
{
    private readonly ILogger<MobFactoryService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly Libertas.Discord.Adventure.Core.Data.IMobPresetService _presetService = presetService ?? throw new ArgumentNullException(nameof(presetService));
    private readonly IRandomNumberGenerator _rng = rng ?? throw new ArgumentNullException(nameof(rng));

    /// <summary>
    /// Creates a <see cref="MobState"/> from a given preset and level, applying stat randomization and scaling.
    /// </summary>
    /// <param name="preset">Mob preset template.</param>
    /// <param name="level">Dungeon level for scaling stats.</param>
    /// <returns>Newly created <see cref="MobState"/>.</returns>
    public Task<MobState> CreateFromPresetAsync(MobPresetDto preset, int level)
    {
        // Randomize stats within preset ranges, then apply level-based scaling for difficulty
        var hp = _rng.Next(preset.MinHealth, preset.MaxHealth + 1) + level * 3;
        var attack = _rng.Next(preset.MinAttackPower, preset.MaxAttackPower + 1) + level * 3;

        var mobState = new MobState
        {
            Name = preset.Name,
            MaxHp = hp,
            CurrentHp = hp,
            AttackPower = new PowerLevel(attack),
            ImageUrl = preset.ImageUrl
        };

        return Task.FromResult(mobState);
    }

    /// <summary>
    /// Creates a random mob for the given level, excluding specified preset IDs.
    /// </summary>
    /// <param name="level">Dungeon level for scaling stats.</param>
    /// <param name="excludePresetIds">Preset IDs to exclude from selection.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Newly created <see cref="MobState"/> or null if no preset is available.</returns>
    public async Task<MobState?> CreateRandomMobForLevelAsync(int level, IReadOnlyCollection<int>? excludePresetIds = null, CancellationToken cancellationToken = default)
    {
        var preset = await _presetService.GetRandomAsync(excludePresetIds, cancellationToken);
        if (preset == null)
        {
            return null;
        }

        return await CreateFromPresetAsync(preset, level);
    }
}