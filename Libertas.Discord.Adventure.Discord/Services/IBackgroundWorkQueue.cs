namespace Libertas.Discord.Adventure.Discord.Services;

/// <summary>
///     Lightweight producer/consumer queue used to offload work from Discord event handlers
///     to background consumers.
/// </summary>
public interface IBackgroundWorkQueue
{
    /// <summary>
    ///     Enqueue a work item to be executed in the background.
    /// </summary>
    ValueTask QueueAsync(Func<CancellationToken, Task> work, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Dequeue a work item. Intended to be used by a dedicated consumer.
    /// </summary>
    ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
}