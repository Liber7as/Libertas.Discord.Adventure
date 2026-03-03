namespace Libertas.Discord.Adventure.Core.GameModels;

/// <summary>
///     Represents an entity that can be alive or dead.
/// </summary>
public interface IAlive
{
    /// <summary>
    ///     Gets a value indicating whether the entity is alive.
    /// </summary>
    bool IsAlive { get; }
}