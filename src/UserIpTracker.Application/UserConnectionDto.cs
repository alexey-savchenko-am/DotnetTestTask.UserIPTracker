namespace UserIpTracker.Application;

public sealed record UserConnectionDto(Guid UserId, string? Ip, DateTime? LastSeenUtc);
