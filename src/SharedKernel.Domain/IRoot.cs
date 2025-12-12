
namespace SharedKernel.Domain;

public interface IRoot<TKey>
    where TKey : IEntityKey<Guid>
{
    public TKey Id { get; }
}
