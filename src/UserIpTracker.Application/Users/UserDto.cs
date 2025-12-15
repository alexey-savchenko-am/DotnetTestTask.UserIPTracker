namespace UserIpTracker.Application.Users;

public sealed record UserDto(
    Guid UserId,
    DateTime LastSeenUtc
);


