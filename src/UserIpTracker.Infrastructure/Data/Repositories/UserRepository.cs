using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Exceptions;
using UserIpTracker.Domain;

namespace UserIpTracker.Infrastructure.Data.Repositories;

internal sealed class UserRepository
        : IUserRepository
{

    private readonly IDbContextFactory<UserDbContext> _factory;

    public UserRepository(IDbContextFactory<UserDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<User?> GetByIdWithConnectionsAsync(UserId id, CancellationToken ct = default)
    {
        await using var dbContext = await _factory
            .CreateDbContextAsync(ct)
            .ConfigureAwait(false);

        return await dbContext.Set<User>()
            .Where(u => u.Id == id)
            .Include(u => u.Connections)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task CreateAsync(User entity, CancellationToken ct = default)
    {
        await using var dbContext = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false); 
        await dbContext.Set<User>().AddAsync(entity, ct).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task StoreAsync(CancellationToken ct = default)
    {
        try
        {
            await using var dbContext = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            throw new PersistenceException(
                "Failed to persist user",
                ex);
        }
    }
}
