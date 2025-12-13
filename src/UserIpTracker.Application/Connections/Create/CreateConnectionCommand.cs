using MediatR;

namespace UserIpTracker.Application.Connections.Create;

public sealed record CreateConnectionCommand(Guid UserId, string Ip, DateTime? TimestampUtc)
    : IRequest<UserConnectionDto?>;
