using Libertas.Discord.Adventure.Core.GameModels;

namespace Libertas.Discord.Adventure.Core.Tests.TestUtilities;

/// <summary>
///     Factory for creating test players and mobs with various configurations.
///     Provides named presets for common test scenarios.
/// </summary>
public static class TestEntityFactory
{
    private static ulong _nextPlayerId = 1;
    private static int _nextMobId = 1;

    /// <summary>
    ///     Resets the ID counters. Call in test setup if needed.
    /// </summary>
    public static void ResetIdCounters()
    {
        _nextPlayerId = 1;
        _nextMobId = 1;
    }

    #region Player Factory Methods

    /// <summary>
    ///     Creates a basic player with default stats.
    /// </summary>
    public static PlayerState CreatePlayer(
        string name,
        int maxHp = 30,
        int currentHp = 30,
        int attackPower = 5,
        int magicPower = 5,
        int speechPower = 5,
        int defensePower = 2,
        bool isBot = false)
    {
        return new PlayerState
        {
            Id = new PlayerId(_nextPlayerId++),
            Name = name,
            MaxHp = maxHp,
            CurrentHp = currentHp,
            AttackPower = new PowerLevel(attackPower),
            MagicPower = new PowerLevel(magicPower),
            SpeechPower = new PowerLevel(speechPower),
            DefensePower = new PowerLevel(defensePower),
            IsBot = isBot
        };
    }

    /// <summary>
    ///     Creates a warrior-type player optimized for physical attacks.
    /// </summary>
    public static PlayerState CreateWarrior(string name = "Warrior")
    {
        return CreatePlayer(
            name,
            40,
            40,
            12,
            2,
            3,
            5
        );
    }

    /// <summary>
    ///     Creates a mage-type player optimized for magic attacks and healing.
    /// </summary>
    public static PlayerState CreateMage(string name = "Mage")
    {
        return CreatePlayer(
            name,
            25,
            25,
            3,
            12,
            5,
            2
        );
    }

    /// <summary>
    ///     Creates a rogue-type player with balanced offense and speech.
    /// </summary>
    public static PlayerState CreateRogue(string name = "Rogue")
    {
        return CreatePlayer(
            name,
            28,
            28,
            8,
            4,
            10,
            3
        );
    }

    /// <summary>
    ///     Creates a cleric-type player optimized for healing.
    /// </summary>
    public static PlayerState CreateCleric(string name = "Cleric")
    {
        return CreatePlayer(
            name,
            30,
            30,
            4,
            14,
            6,
            4
        );
    }

    /// <summary>
    ///     Creates a tank-type player with high HP and defense.
    /// </summary>
    public static PlayerState CreateTank(string name = "Tank")
    {
        return CreatePlayer(
            name,
            50,
            50,
            6,
            2,
            2,
            8
        );
    }

    /// <summary>
    ///     Creates a player with critically low HP (for testing run/pray mechanics).
    /// </summary>
    public static PlayerState CreateInjuredPlayer(string name = "Injured", int currentHp = 5)
    {
        return CreatePlayer(
            name,
            30,
            currentHp,
            5,
            5,
            5,
            2
        );
    }

    /// <summary>
    ///     Creates a standard balanced party of 4 players.
    /// </summary>
    public static List<PlayerState> CreateStandardParty()
    {
        ResetIdCounters();
        return
        [
            CreateWarrior("Alice"),
            CreateMage("Bob"),
            CreateRogue("Carol"),
            CreateCleric("Dave")
        ];
    }

    /// <summary>
    ///     Creates a solo player (to test bot injection).
    /// </summary>
    public static List<PlayerState> CreateSoloPlayer(string name = "Hero")
    {
        ResetIdCounters();
        return [CreatePlayer(name, 35, attackPower: 8, magicPower: 6, speechPower: 5, defensePower: 4)];
    }

    #endregion

    #region Mob Factory Methods

    /// <summary>
    ///     Creates a basic mob with specified stats.
    /// </summary>
    public static MobState CreateMob(
        string name,
        int maxHp = 30,
        int currentHp = 30,
        int attackPower = 5)
    {
        return new MobState
        {
            Id = new MobId(_nextMobId++),
            Name = name,
            MaxHp = maxHp,
            CurrentHp = currentHp,
            AttackPower = new PowerLevel(attackPower)
        };
    }

    /// <summary>
    ///     Creates a weak mob (good for early levels or quick tests).
    /// </summary>
    public static MobState CreateWeakMob(string name = "Goblin")
    {
        return CreateMob(name, 20, 20, 4);
    }

    /// <summary>
    ///     Creates a standard mob (balanced challenge).
    /// </summary>
    public static MobState CreateStandardMob(string name = "Orc")
    {
        return CreateMob(name, 40, 40, 8);
    }

    /// <summary>
    ///     Creates a tough mob (mini-boss level).
    /// </summary>
    public static MobState CreateToughMob(string name = "Troll")
    {
        return CreateMob(name, 80, 80, 12);
    }

    /// <summary>
    ///     Creates a boss mob (high HP and damage).
    /// </summary>
    public static MobState CreateBossMob(string name = "Dragon")
    {
        return CreateMob(name, 150, 150, 20);
    }

    /// <summary>
    ///     Creates a swarm of weak mobs.
    /// </summary>
    public static List<MobState> CreateMobSwarm(int count = 3, string baseName = "Rat")
    {
        return [.. Enumerable.Range(1, count).Select(i => CreateMob($"{baseName} #{i}", 15, 15, 3))];
    }

    /// <summary>
    ///     Creates a level-appropriate mob based on dungeon level.
    ///     HP and attack scale with level.
    /// </summary>
    public static MobState CreateScaledMob(int level, string name = "Monster")
    {
        var hp = 20 + level * 5;
        var attack = 4 + level * 2;
        return CreateMob(name, hp, hp, attack);
    }

    #endregion
}