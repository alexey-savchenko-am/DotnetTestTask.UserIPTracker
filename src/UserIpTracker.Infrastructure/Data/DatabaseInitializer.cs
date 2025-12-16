using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserIpTracker.Infrastructure.Data;

namespace FormCollector.Infrastructure.Data;

internal sealed class DatabaseInitializer
    : IDatabaseInitializer
{
    private readonly UserDbContext _dbContext;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        UserDbContext dbContext, 
        ILogger<DatabaseInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task InitializeAsync(bool recreateDatabase = false, CancellationToken ct = default)
    {
        _logger.LogInformation("Applying migrations for {DbContext}...", typeof(UserDbContext).Name);

        try
        {
            if (recreateDatabase)
            {
                await _dbContext.Database.EnsureDeletedAsync(ct).ConfigureAwait(false);
            }
            await _dbContext.Database.MigrateAsync(ct).ConfigureAwait(false);

            _logger.LogInformation("✅ Migrations applied successfully for {DbContext}.", typeof(UserDbContext).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Migration failed for {DbContext}.", typeof(UserDbContext).Name);
            throw;
        }
    }
}

