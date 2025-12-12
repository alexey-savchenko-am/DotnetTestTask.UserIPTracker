namespace SharedKernel.Domain;

public interface IEntityKey<TKey>
{
    public TKey Key { get; }
}

public interface IEntityKey<TSelf, TKey>
    : IEntityKey<TKey>
    where TSelf : class
{
    abstract static TSelf Create();
}
