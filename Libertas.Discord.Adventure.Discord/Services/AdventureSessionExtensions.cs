namespace Libertas.Discord.Adventure.Discord.Services;

/// <summary>
///     Convenience extensions for acquiring and using per-session locks safely.
/// </summary>
public static class AdventureSessionExtensions
{
    /// <summary>
    ///     Lock the session identified by <paramref name="channelId" /> and execute the provided function.
    ///     Throws if the session is not found.
    /// </summary>
    public static async Task LockAsync(this AdventureSessionManager manager, ulong channelId, Func<AdventureSession, CancellationToken, Task> lockFunc, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (!manager.Sessions.TryGetValue(channelId, out var session))
        {
            throw new InvalidOperationException("Session not found.");
        }

        await session.LockAsync(async ct => await lockFunc(session, ct), cancellationToken);
    }

    /// <summary>
    ///     Lock the given session and execute the provided function while holding the session semaphore.
    /// </summary>
    public static async Task LockAsync(this AdventureSession session, Func<CancellationToken, Task> lockFunc, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await session.Semaphore.WaitAsync(cancellationToken);

        try
        {
            await lockFunc(cancellationToken);
        }
        finally
        {
            session.Semaphore.Release();
        }
    }
}