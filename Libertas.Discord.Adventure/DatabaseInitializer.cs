using Libertas.Discord.Adventure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Libertas.Discord.Adventure;

/// <summary>
/// Hosted service responsible for ensuring the application's database schema is applied on startup.
/// Runs EF Core migrations. Failures will stop host startup.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="DatabaseInitializer"/>.
/// </remarks>
/// <param name="dbContextFactory">Factory used to create <see cref="AdventureContext"/> instances.</param>
/// <param name="logger">Logger used to report status and failures.</param>
public class DatabaseInitializer(IDbContextFactory<AdventureContext> dbContextFactory, ILogger<DatabaseInitializer> logger) : IHostedService
{
    // Factory to create short-lived DbContext instances on demand
    private readonly IDbContextFactory<AdventureContext> _dbContextFactory = dbContextFactory;

    // Logger for reporting progress and errors
    private readonly ILogger<DatabaseInitializer> _logger = logger;

    /// <summary>
    /// Called by the host to start this hosted service. Applies any pending EF Core migrations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token provided by the host.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Log that initialization is starting
            _logger.LogInformation("Initializing database...");

            // Create a DbContext instance from the factory using the provided cancellation token
            await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            // Apply pending migrations to bring the database schema up to date.
            // Use MigrateAsync so migrations are applied in production scenarios.
            await db.Database.MigrateAsync(cancellationToken);

            // Log successful completion
            _logger.LogInformation("Database initialization complete.");
        }
        catch (Exception ex)
        {
            // Log failure with exception details
            _logger.LogError(ex, "Database initialization failed");

            // Rethrow to ensure host startup fails and the issue is visible
            throw;
        }
    }

    /// <summary>
    /// Called by the host to stop this hosted service. No action required for database initialization.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token provided by the host.</param>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Nothing to dispose or stop explicitly; return completed task.
        return Task.CompletedTask;
    }
}