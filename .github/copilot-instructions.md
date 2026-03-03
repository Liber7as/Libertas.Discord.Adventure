# Copilot Instructions (Libertas.Discord.Adventure)

## Copilot Guidance
When working on this project, please follow these conventions:
- **One type per file**: Every class, interface, record, or enum must be declared in its own file, and the filename must match the type name. Do not place multiple types in a single file. This keeps the codebase organized and easy to navigate.
- `PlayerState` and `MobState` are **mutable classes** — modify properties directly (e.g., `player.CurrentHp = newHp`). Do not use immutable record or `with`-expression patterns for state models.
- Prefer injecting dependencies (e.g., `IRandomNumberGenerator`, `IBackgroundWorkQueue`, `ILogger`, `IBotService`) rather than creating new instances.
- Keep Discord command handlers and event handlers lightweight — enqueue heavy CPU/IO work via `IBackgroundWorkQueue`.
- Centralize action effects, scaling, loot, and narrative text in `ActionResolutionService` and `ActionLocalizationService`.
- Use structured logging with `ILogger` for important events and errors.
- When extending gameplay, update scaling/loot/healing logic in `ActionResolutionService` or `GameEngine` as needed.
- Configuration values should be placed in settings classes (e.g., `BotSettings`, `LocalizationSettings`) and bound via `appsettings.json`.
- **Keep Instructions Updated**: After every change to the codebase, reconsider these instructions and ensure they are updated to reflect the latest project structure and conventions.
- **Build and Test**: After every change, ensure the solution builds successfully and all tests pass.
- **Review Existing Files**: Whenever encountering an issue, review existing files to see if they provide solutions or established patterns to follow. Always adhere to the guidelines and patterns set by other files in the project.
- **Make No Assumptions**: Never assume package versions, APIs, or conventions are static. Always verify accuracy using web searches and official documentation. Document this verification process in your reasoning and instructions.
- **Documentation & Comments**: Always include clear XML summaries on public types and members, and concise inline comments for complex logic. Comments should explain the rationale ('why'), not just the mechanics ('what'), unless the code is non-obvious. This matches project conventions and should be followed for all future documentation work. Keep GitHub CoPilot Instructions up to date alongside code changes.

### Generated XML documentation files
- To get the latest API documentation for any assembly, build the solution. The build generates XML documentation files to each project's folder by the project name, for example `Libertas.Discord.Adventure.Core/Libertas.Discord.Adventure.Core.xml`.
- Build locally before reading API docs so the XML reflects current source comments.
- When updating or adding XML comments in source, rebuild the solution to regenerate these XML files before consuming them for documentation or tooling.

## Quick Start & Developer Workflows (Windows)
- **Build**: `dotnet build Libertas.Discord.Adventure.sln`
- **Run bot**: `dotnet run --project Libertas.Discord.Adventure/Libertas.Discord.Adventure.csproj`
- **Tests** (NUnit): `dotnet test Libertas.Discord.Adventure.Core.Tests/Libertas.Discord.Adventure.Core.Tests.csproj`
- **EF Migrations**:
  - Add migration: `./dotnet-add-migration.ps1 "<MigrationName>"`
  - Update database: `./dotnet-update-database.ps1`
- **Configure secrets**:dotnet user-secrets set "Discord:Token" "<token>" --project Libertas.Discord.Adventure/Libertas.Discord.Adventure.csproj
## Big Picture Overview
Libertas.Discord.Adventure is a .NET-based Discord bot for cooperative turn-based combat. It features:
- **Libertas.Discord.Adventure** — Executable host (config + DI composition).
- **Libertas.Discord.Adventure.Discord** — Discord.Net integration (command modules, per-channel sessions).
- **Libertas.Discord.Adventure.Core** — Game logic (engine, action resolution/localization).
- **Libertas.Discord.Adventure.Data** — EF Core context/entities/migrations.

## Runtime & Architecture Flow
- **Host Entry**: `Program.cs` initializes DI, runs EF migrations, and starts the bot.
- **Discord Lifecycle**: `DiscordBackgroundService` logs in, loads command modules, and processes events via `DiscordEventHandler`.
- **Interaction Handling**: Button clicks are processed in the background via `IBackgroundWorkQueue`.
- **Session Management**: `AdventureSessionManager` tracks per-channel sessions with thread-safe locks.

## Configuration
- **Secrets**: Use `dotnet user-secrets` for sensitive data (e.g., Discord token).
- **Round Timer**: Hardcoded to 60 seconds per round.
- **Database**: SQLite file (`Data Source=adventure.db`).

## Extending Discord Commands
- Commands are defined in `Libertas.Discord.Adventure.Discord/Modules/`.
- `a start` initiates the adventure loop (timed rounds + button interactions).
- Use `IBackgroundWorkQueue` for CPU/IO-heavy tasks.

## Available Commands
| Module           | Command               | Description                                      |
|------------------|-----------------------|--------------------------------------------------|
| AdventureModule  | `a start`            | Starts a button-driven adventure in the channel. |
| AdventureModule  | `a stats`            | View your skills, XP, gold, and lifetime stats.  |
| AdventureModule  | `a simulate`         | Runs a headless combat simulation for testing.   |
| AdminModule      | `admin say <message>`| Echoes the provided message back to the channel. |

## Game Logic Conventions
- **Round Loop**: `IGameEngine.ExecuteRoundAsync(...)` processes actions and returns results.
- **Action Resolution**: Uses Strategy pattern — `ActionResolutionService` delegates to `IPlayerActionHandler` implementations.
- **Combat Calculations**: Centralized in `ICombatCalculator` with configurable settings via `CombatSettings`.
- **State Models**: Mutable `class` types (`PlayerState`, `MobState`) — modify properties directly during combat.
- **Randomness**: Abstracted via `IRandomNumberGenerator`.

## Refactored Game Engine
The `GameEngine` has been refactored to delegate responsibilities to smaller, focused components:
- **PlayerPhaseProcessor**: Handles all player and bot actions during the player phase.
- **BotPhaseProcessor**: Ensures the party meets the minimum size by injecting bots.
- **MobPhaseProcessor**: Handles all mob actions during the mob phase.
- **RoundSummaryGenerator**: Generates the round summary, including XP and gold earned.

This refactor improves maintainability and separates concerns, making the codebase easier to extend and debug.

## Bot System
AI companion bots automatically join the party to ensure a minimum party size for solo or small-group play.

### How It Works
- **Automatic Injection**: At the start of each round, `GameEngine` checks if the party is below `MinimumPartySize` and adds bots via `IBotService`.
- **Stat Scaling**: Bot stats are based on the current dungeon level with configurable variance (±`StatLevelVariance`), allowing players to get lucky or unlucky with companion quality.
- **Bot AI**: Bots use simple decision logic:
  - 40% chance to heal if an ally is below 50% HP
  - 30% chance to pray if the bot itself is below 30% HP
  - Otherwise: 70% attack, 20% magic, 10% talk
- **Loot Exclusion**: Bots participate in combat but do **not** receive loot rewards — all gold goes to human players.
- **Persistence**: Bots persist between rounds (same bots stay until they die or the session ends).
- **No Replacement**: Dead bots are **not** replaced — the party is capped at `MinimumPartySize` total bots.
- **Abandonment**: If all human players die or flee, remaining bots automatically flee and the session ends.

### Configuration (`appsettings.json` → `Bot` section)
| Setting             | Default | Description                                              |
|---------------------|---------|----------------------------------------------------------|
| `MinimumPartySize`  | 4       | Minimum players (including bots) required for combat.    |
| `StatLevelVariance` | 2       | Variance (±) applied to bot level relative to dungeon level. |
| `BaseHp`            | 20      | Base HP for bots.                                        |
| `HpPerLevel`        | 5       | Additional HP per level.                                 |
| `BasePower`         | 1       | Base power for all stats.                                |
| `PowerPerLevel`     | 2       | Additional power per level.                              |
| `MinimumBotLevel`   | 1       | Floor for bot level (prevents negative/zero levels).    |
| `BotNames`          | [...]   | Thematic names for bots (random selection, no repeats). |

### Extending Bots
- To add new bot names, edit `BotNames` in `appsettings.json`.
- To change bot AI behavior, modify `BotService.DecideBotAction(...)`.
- To change stat scaling formulas, modify `BotService.CreateBot(...)` and `BotSettings`.

## Player Progression System
Skill-based progression where players level up individual skills by using them in combat.

### Skills
| Skill   | Levels By             | Affects                        |
|---------|----------------------|--------------------------------|
| Attack  | Using Attack action  | Physical damage dealt          |
| Magic   | Using Magic/Heal     | Magic damage and heal amount   |
| Speech  | Using Talk action    | Diplomacy success chance       |
| Defense | Taking damage        | HP pool and damage reduction   |

### Stat Formulas
- **HP**: `BaseHp (20) + (DefenseLevel - 1) × 5`
- **AttackPower**: `BaseAttack (5) + (AttackLevel - 1) × 2`
- **MagicPower**: `BaseMagic (5) + (MagicLevel - 1) × 2`
- **SpeechPower**: `BaseSpeech (5) + (SpeechLevel - 1) × 2`
- **DefensePower**: `BaseDefense (2) + (DefenseLevel - 1) × 1`

### Skill XP
- **XP per skill use**: `BaseSkillXp (10) + DungeonLevel × 2`
- **Defense XP**: `DamageTaken × 0.5` (rounded up)
- **XP to next level**: `CurrentLevel × 50`
- **Max skill level**: 99

### Configuration (`appsettings.json` → `Progression` section)
| Setting               | Default | Description                              |
|-----------------------|---------|------------------------------------------|
| `BaseHp`              | 20      | Starting HP at Defense level 1.          |
| `HpPerDefenseLevel`   | 5       | HP gained per Defense level.             |
| `AttackPerLevel`      | 2       | Attack power per Attack level.           |
| `MagicPerLevel`       | 2       | Magic power per Magic level.             |
| `SpeechPerLevel`      | 2       | Speech power per Speech level.           |
| `DefensePerLevel`     | 1       | Defense power per Defense level.         |
| `BaseSkillXp`         | 10      | Base XP for using a skill.               |
| `SkillXpPerDungeonLevel` | 2    | Bonus XP per dungeon level.              |
| `SkillXpPerLevel`     | 50      | XP required per skill level.             |
| `MaxSkillLevel`       | 99      | Maximum level for any skill.             |

### Persistence
- **Player Entity**: Stored in SQLite database via EF Core.
- **Auto-created**: Players are created on first button click (no registration).
- **Progress saved**: After each session, XP and gold are persisted.

## Data & EF Core
- **Context**: `AdventureContext` applies configurations via `ApplyConfigurationsFromAssembly(...)`.
- **Entities**: `Player` (progression), `MobPreset` (mob templates).
- **Migrations**: Managed via EF Core scripts. Auto-seeded mob presets on fresh databases via migrations.

## Gameplay Overview
Cooperative turn-based adventure where channel members team up via buttons to battle endless waves of mobs. Features real-time coordination (live action counts), crits, creative actions, shared loot, and exponential scaling for challenging replayability.

### Round Flow
1. **Session Start**: `a start` begins level 1 with a random mob (unused preset preferred).
2. **Round Info**: Posts teaser (round 1) or mob details (name, HP, image gallery if available).
3. **Combat Buttons**: Persistent message with 6 action buttons (emojis + live choice counts).
4. **Player Actions**: Players join implicitly on first button click. Actions recorded (ephemeral confirmation).
5. **Round Resolution**: Results posted (logs in code block, player/mob status with gold earned, next timestamp). Combat button message temporarily deleted during resolution.
6. **Progression**: Mob defeat → loot split + skill XP, level up, new mob. Continues until wipe or empty.

### Player Actions
| Action  | Description                          | XP Gained     | Details/Formulas                              | Target                  |
|---------|--------------------------------------|---------------|-----------------------------------------------|-------------------------|
| Attack  | Physical damage to a mob.            | Attack XP     | 15% crit chance (2x damage).                 | Random mob.             |
| Magic   | Magic damage to a mob.               | Magic XP      | 15% crit chance.                             | Random mob.             |
| Heal    | Heal an ally.                        | Magic XP      | `MagicPower / 2 + random(5–10)` HP.          | Random injured ally.    |
| Pray    | Chance to instant-kill mob; else self-heal. | Magic XP | `clamp(10 + (20 - level), 5–30%)` chance.    | Self or random mob.     |
| Talk    | Attempt to defeat a mob.             | Speech XP     | `SpeechPower / (Mob AttackPower + level/2)`. | Random mob.             |
| Run     | Attempt to flee.                     | None          | `clamp(40% + bonuses, 10–85%)`.              | Self.                   |
| (Damage)| Taking damage awards Defense XP.     | Defense XP    | `DamageTaken × 0.5` (rounded up).            | N/A                     |

### Edge Cases
- **Mid-Round Join**: Players can join and act mid-round.
- **Idle Players**: No action recorded if idle.
- **Session Cleanup**: Ends only on wipe or zero players (no inactivity timeout).
- Buttons always available; supports mid-round joins/actions.

## Testing, Logging & Session Management
- **Tests**: Comprehensive test suite in `Libertas.Discord.Adventure.Core.Tests`.
- **Logging**: Use `ILogger<T>` for structured logging with consistent patterns (see below).
- **Session State**: Managed by `AdventureSessionManager` (thread-safe).
- **Session End Persistence**: `EndSessionAsync` automatically saves all player progress (XP, gold, kills, deaths) and returns `SessionEndResult` with level-up information.

### Logging Standards
Follow these conventions for consistent, informative logging:

| Level | Usage | Examples |
|-------|-------|----------|
| `Trace` | Per-action details, AI decisions | Bot action choices, individual damage rolls |
| `Debug` | Per-round summaries, state changes | Round start/end, bot injection counts |
| `Information` | Session lifecycle, significant events | Session start/end, level-ups, bot generation |
| `Warning` | Recoverable issues, unexpected input | Unknown button IDs, missing player actions |
| `Error` | Failures that affect functionality | Database errors, failed Discord API calls |
| `Critical` | Startup failures, configuration errors | Missing action handlers, invalid settings |

**Logging Format:**
```csharp
// Use structured logging with named parameters
_logger.LogInformation(
    "Player {PlayerName} ({PlayerId}) earned {Gold:F0} gold at level {Level}",
    player.Name, player.Id.Value, player.GoldEarned, level);

// Include context for debugging
_logger.LogDebug(
    "Bot {BotName} choosing {Action} (injured allies: {InjuredCount}, self HP: {Percent}%)",
    bot.Name, action, injuredCount, hpPercent);
```

## State Model Architecture
- `PlayerState` and `MobState` are **mutable classes** (not records) for simpler in-combat state updates.
- State is always modified in place via property setters (e.g., `player.CurrentHp = newHp`).
- Do not use immutable record or `with`-expression patterns for state models.
- State is modified in place during combat rounds for better performance and clearer code.

## Formatting & Analyzer Enforcement

- Ensure a single source of truth for code style by keeping an `.editorconfig` at the repository root
- Recommended developer workflow (local):
  - Install the `dotnet-format` tool (global or repo tool):
    - Global: `dotnet tool install -g dotnet-format`
    - Or add a `dotnet-tools.json` manifest to the repo for a reproducible toolset.
  - Apply formatting and analyzer fixes before committing:
    - `dotnet format`
    - `dotnet format style`
    - `dotnet format analyzers`
  - Rebuild and test after fixes: `dotnet build` and `dotnet test`.
- To enforce formatting in pre-commit, add a lightweight hook that runs:
  - `dotnet format --verify-no-changes` (fail the commit when style violations exist).
- Address analyzer diagnostics that lack automated fixes manually and aim to keep warnings minimal

These practices reduce stylistic churn, minimize analyzer warnings, and keep the repo consistent across contributors and tools.

