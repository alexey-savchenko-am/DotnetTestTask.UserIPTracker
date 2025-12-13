namespace UserIpTracker.Application.Connections;

public sealed record UserConnectionDto(Guid UserId, string Ip, DateTime LastSeenUtc);
