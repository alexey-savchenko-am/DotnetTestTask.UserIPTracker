using MediatR;

namespace UserIpTracker.Application.Connections.GetAll;

public sealed record GetAllConnectionsQuery(Guid UserId)
    : IRequest<List<UserConnectionDto>>;
