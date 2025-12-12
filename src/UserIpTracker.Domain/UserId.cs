using SharedKernel.Domain;

namespace UserIpTracker.Domain;

public sealed record UserId(Guid Id)
    : IEntityKey<UserId, Guid>
{
    public Guid Key => Id;

    public static UserId Create() => new(Guid.NewGuid());
}
