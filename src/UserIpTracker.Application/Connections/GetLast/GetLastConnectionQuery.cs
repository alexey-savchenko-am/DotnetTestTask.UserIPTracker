using MediatR;

namespace UserIpTracker.Application.Connections.GetLast;

public sealed record GetLastConnectionQuery(Guid UserId) 
    : IRequest<UserConnectionDto?>;
