namespace UserIpTracker.Infrastructure.Data;

public interface IDatabaseInitializer
{
    Task InitializeAsync(bool recreateDatabase = false, CancellationToken ct = default);
}
