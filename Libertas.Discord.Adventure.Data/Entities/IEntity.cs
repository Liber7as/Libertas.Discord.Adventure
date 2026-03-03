namespace Libertas.Discord.Adventure.Data.Entities;

/// <summary>
///     Marker interface for EF entities exposing an integer Id property.
/// </summary>
public interface IEntity
{
    /// <summary>
    ///     Primary key identifier.
    /// </summary>
    int Id { get; set; }
}