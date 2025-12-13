namespace UserIpTracker.Application.Connections;

public sealed record UserDto(
    Guid UserId, 
    UserConnectionDto LastConnection, 
    IEnumerable<UserConnectionDto> Connections
);


