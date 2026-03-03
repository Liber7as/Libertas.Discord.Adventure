using Discord.WebSocket;
using Libertas.Discord.Adventure.Core.Data;
using Libertas.Discord.Adventure.Core.GameModels;
using Libertas.Discord.Adventure.Core.Services;
using Microsoft.Extensions.Logging;
// using Libertas.Discord.Adventure.Data.Entities; // no longer needed
// using Libertas.Discord.Adventure.Discord.Data; // no longer needed

namespace Libertas.Discord.Adventure.Discord.Data;

/// <summary>
///     Implementation of <see cref="IPlayerService" /> integrating database persistence with combat state.
///     Handles player creation, stat calculation, and progress saving.
/// </summary>
/// <remarks>
///     Constructs the player service with required dependencies.
/// </remarks>
/// <param name="playerRepository">Repository for player persistence.</param>
/// <param name="progressionService">Service for skill/stat calculations.</param>
/// <param name="logger">Logger for structured logging.</param>
public class PlayerService(
    IPlayerRepository playerRepository,
    IPlayerProgressionService progressionService,
    ILogger<PlayerService> logger) : IPlayerService
{
    private readonly ILogger<PlayerService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IPlayerRepository _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    private readonly IPlayerProgressionService _progressionService = progressionService ?? throw new ArgumentNullException(nameof(progressionService));

    /// <inheritdoc />
    public Task<PlayerState> GetOrCreateAsync(SocketUser user, CancellationToken cancellationToken = default)
    {
        return GetOrCreateAsync(user.Id, user.Username, cancellationToken);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Gets or creates a player by Discord user ID and username, then calculates combat stats.
    /// </summary>
    /// <param name="userId">Discord user ID.</param>
    /// <param name="username">Discord username.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Player combat state.</returns>
    public async Task<PlayerState> GetOrCreateAsync(ulong userId, string username, CancellationToken cancellationToken = default)
    {
        // Load or create player in database via core repository
        var persisted = await _playerRepository.GetOrCreateAsync(new PlayerId(userId), username, cancellationToken);

        // Calculate combat stats from skill levels for the current session
        var skillLevels = persisted.Skills;
        var stats = _progressionService.CalculateStats(skillLevels);

        // Apply calculated combat stats to the session state
        persisted.MaxHp = stats.MaxHp;
        persisted.CurrentHp = stats.MaxHp;
        persisted.AttackPower = new PowerLevel(stats.AttackPower);
        persisted.MagicPower = new PowerLevel(stats.MagicPower);
        persisted.SpeechPower = new PowerLevel(stats.SpeechPower);
        persisted.DefensePower = new PowerLevel(stats.DefensePower);

        _logger.LogInformation(
            "Loaded player {PlayerId} ({Username}) - Combat Lv.{CombatLevel} (ATK:{Atk} MAG:{Mag} SPE:{Spe} DEF:{Def})",
            persisted.Id.Value, persisted.Name, skillLevels.CombatLevel,
            skillLevels.AttackLevel, skillLevels.MagicLevel, skillLevels.SpeechLevel, skillLevels.DefenseLevel);

        return persisted;
    }

    /// <inheritdoc />
    /// <summary>
    ///     Saves player progress after a session, updating XP, gold, kills, deaths, and detecting level-ups.
    /// </summary>
    /// <param name="playerState">Player combat state.</param>
    /// <param name="dungeonLevel">Dungeon level reached.</param>
    /// <param name="died">True if player died during session.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing any skill level-ups.</returns>
    public async Task<SaveProgressResult> SaveProgressAsync(PlayerState playerState, int dungeonLevel, bool died, CancellationToken cancellationToken = default)
    {
        var levelUps = new List<SkillLevelUp>();

        if (playerState.IsBot)
        {
            // Don't persist bot progress
            return new SaveProgressResult(levelUps);
        }

        // Load persisted session state
        var persisted = await _playerRepository.GetByIdAsync(playerState.Id, cancellationToken);
        if (persisted == null)
        {
            _logger.LogWarning("Cannot save progress for unknown player {PlayerId}", playerState.Id);
            return new SaveProgressResult(levelUps);
        }

        // Store old levels for level-up detection
        var oldAttackLevel = persisted.Skills.AttackLevel;
        var oldMagicLevel = persisted.Skills.MagicLevel;
        var oldSpeechLevel = persisted.Skills.SpeechLevel;
        var oldDefenseLevel = persisted.Skills.DefenseLevel;

        // Compute new XP totals (do not persist yet)
        var newAttackTotal = persisted.AttackXpTotal + playerState.AttackXpEarned;
        var newMagicTotal = persisted.MagicXpTotal + playerState.MagicXpEarned;
        var newSpeechTotal = persisted.SpeechXpTotal + playerState.SpeechXpEarned;
        var newDefenseTotal = persisted.DefenseXpTotal + playerState.DefenseXpEarned;

        // Recalculate skill levels based on new XP totals
        var newAttackLevel = _progressionService.GetSkillLevelForXp(newAttackTotal);
        var newMagicLevel = _progressionService.GetSkillLevelForXp(newMagicTotal);
        var newSpeechLevel = _progressionService.GetSkillLevelForXp(newSpeechTotal);
        var newDefenseLevel = _progressionService.GetSkillLevelForXp(newDefenseTotal);

        // Detect level-ups for each skill
        if (newAttackLevel > oldAttackLevel)
        {
            levelUps.Add(new SkillLevelUp("Attack", oldAttackLevel, newAttackLevel));
        }

        if (newMagicLevel > oldMagicLevel)
        {
            levelUps.Add(new SkillLevelUp("Magic", oldMagicLevel, newMagicLevel));
        }

        if (newSpeechLevel > oldSpeechLevel)
        {
            levelUps.Add(new SkillLevelUp("Speech", oldSpeechLevel, newSpeechLevel));
        }

        if (newDefenseLevel > oldDefenseLevel)
        {
            levelUps.Add(new SkillLevelUp("Defense", oldDefenseLevel, newDefenseLevel));
        }

        // Prepare persisted state for update: set new levels and include session deltas
        persisted.Skills = new SkillLevels(newAttackLevel, newMagicLevel, newSpeechLevel, newDefenseLevel);
        persisted.AttackXpEarned = playerState.AttackXpEarned;
        persisted.MagicXpEarned = playerState.MagicXpEarned;
        persisted.SpeechXpEarned = playerState.SpeechXpEarned;
        persisted.DefenseXpEarned = playerState.DefenseXpEarned;
        persisted.GoldEarned = playerState.GoldEarned;
        persisted.MobsKilled = playerState.MobsKilled;

        if (dungeonLevel > persisted.HighestDungeonLevel)
        {
            persisted.HighestDungeonLevel = dungeonLevel;
        }

        if (died)
        {
            persisted.TotalDeaths++;
        }

        await _playerRepository.UpdateAsync(persisted, cancellationToken);

        _logger.LogInformation(
            "Saved progress for {PlayerId}: +{AttackXp} ATK XP, +{MagicXp} MAG XP, +{SpeechXp} SPE XP, +{DefenseXp} DEF XP, +{Gold:F0} gold, +{Kills} kills, {LevelUps} level-ups",
            playerState.Id, playerState.AttackXpEarned, playerState.MagicXpEarned,
            playerState.SpeechXpEarned, playerState.DefenseXpEarned, playerState.GoldEarned, playerState.MobsKilled, levelUps.Count);

        return new SaveProgressResult(levelUps);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Gets the persisted player state by ID.
    /// </summary>
    /// <param name="playerId">Player ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The player state or null.</returns>
    public async Task<PlayerState?> GetPlayerDataAsync(ulong playerId, CancellationToken cancellationToken = default)
    {
        return await _playerRepository.GetByIdAsync(new PlayerId(playerId), cancellationToken);
    }
}