using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Exceptions;
using UserIpTracker.Domain;

namespace UserIpTracker.Infrastructure.Data.Repositories;

internal sealed class UserRepository(DbContext dbContext)
        : IUserRepository
{
    public async Task<User?> GetByIdWithConnectionsAsync(UserId id, CancellationToken ct = default)
    {
       var user = await dbContext.Set<User>()
            .Where(u => u.Id == id)
            .Include(u => u.Connections)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        return user;
    }

    public async Task CreateAsync(User entity, CancellationToken ct = default)
    {
       await dbContext.Set<User>().AddAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task StoreAsync(CancellationToken ct = default)
    {
        try
        {
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
