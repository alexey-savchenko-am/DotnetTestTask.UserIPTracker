using MediatR;

namespace UserIpTracker.Application.Connections.GetLast;

public sealed record GetLastConnectionByIpQuery(Guid UserId, string Ip) 
    : IRequest<UserConnectionDto?>;
