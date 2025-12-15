
using MediatR;

namespace UserIpTracker.Application.Users.GetAll;

public sealed record GetAllUsersByIpQuery(string IpKeyword)
    : IRequest<List<UserDto>>;
