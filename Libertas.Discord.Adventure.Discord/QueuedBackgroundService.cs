using Libertas.Discord.Adventure.Discord.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Libertas.Discord.Adventure.Discord;

/// <summary>
///     Background service that processes queued work items from <see cref="IBackgroundWorkQueue" />.
///     Ensures CPU/IO-heavy tasks are executed outside Discord event handlers for responsiveness.
/// </summary>
/// <remarks>
///     Uses a single-threaded loop to dequeue and execute work items, logging errors and shutdown events.
/// </remarks>
/// <remarks>
///     Constructs the background service with the required work queue and logger.
/// </remarks>
/// <param name="queue">Work queue to process.</param>
/// <param name="logger">Logger for structured logging.</param>
public sealed class QueuedBackgroundService(IBackgroundWorkQueue queue, ILogger<QueuedBackgroundService> logger) : BackgroundService
{
    private readonly ILogger<QueuedBackgroundService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IBackgroundWorkQueue _queue = queue ?? throw new ArgumentNullException(nameof(queue));

    /// <summary>
    ///     Main execution loop for the background service. Dequeues and runs work items until shutdown.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token for graceful shutdown.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued background service is starting.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Func<CancellationToken, Task> work;

                try
                {
                    // Wait for the next work item from the queue
                    work = await _queue.DequeueAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Shutdown requested
                    break;
                }

                // Run the work item in a background task
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await work(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        // Log any unhandled exceptions from the work item
                        _logger.LogError(ex, "Unhandled exception executing background work item.");
                    }
                }, stoppingToken);
            }
        }
        finally
        {
            _logger.LogInformation("Queued background service is stopping.");
        }
    }
}