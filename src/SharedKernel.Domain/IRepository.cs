namespace SharedKernel.Domain;

public interface IRepository<TRoot, TKey>
    where TKey : IEntityKey<Guid>
    where TRoot : class, IRoot<TKey>
{
    Task CreateAsync(TRoot entity, CancellationToken ct = default);
    Task StoreAsync(CancellationToken ct = default);
}
