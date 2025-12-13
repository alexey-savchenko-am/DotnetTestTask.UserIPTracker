using SharedKernel.Domain;

namespace UserIpTracker.Domain;

public interface IUserRepository
    : IRepository<User, UserId>
{
    Task<User?> GetByIdWithConnectionsAsync(UserId id, CancellationToken ct = default); 
}
