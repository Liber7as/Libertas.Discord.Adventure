using System.Threading.Channels;

namespace Libertas.Discord.Adventure.Discord.Services;

/// <summary>
/// Simple bounded background work queue used to offload CPU/IO work from Discord event handlers.
/// Stores delegates of the form <c>Func&lt;CancellationToken, Task&gt;</c> and exposes dequeue/queue operations.
/// </summary>
/// <remarks>
/// The queue uses a single reader / multiple writer bounded channel to provide backpressure
/// and to ensure tasks are processed sequentially by a single background worker.
/// </remarks>
public sealed class BackgroundWorkQueue : IBackgroundWorkQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _channel;
    /// <summary>
    /// Creates a new bounded <see cref="BackgroundWorkQueue"/> with the provided capacity.
    /// </summary>
    /// <param name="capacity">Maximum number of queued work items before backpressure is applied.</param>
    public BackgroundWorkQueue(int capacity = 100)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        };

        _channel = Channel.CreateBounded<Func<CancellationToken, Task>>(options);
    }

    /// <summary>
    /// Enqueues a delegate to be executed by a background consumer. This method waits
    /// when the channel is full to provide backpressure.
    /// </summary>
    public async ValueTask QueueAsync(Func<CancellationToken, Task> work, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);

        await _channel.Writer.WriteAsync(work, cancellationToken);
    }

    /// <summary>
    /// Dequeues the next work delegate. This call blocks until an item becomes available
    /// or the provided cancellation token is triggered.
    /// </summary>
    public async ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken = default)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }
}