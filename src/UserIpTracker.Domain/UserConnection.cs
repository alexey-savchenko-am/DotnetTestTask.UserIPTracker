using System.Net;

namespace UserIpTracker.Domain;

public sealed class UserConnection 
    : IEquatable<UserConnection>
{
    private DateTime? _lastSeenUtc;

    public IPAddress Ip { get; }
    public DateTime LastSeenUtc => _lastSeenUtc ?? throw new InvalidOperationException("LastSeen missing");

    private UserConnection(IPAddress ip, DateTime lastSeen)
    {
        Ip = ip;
        _lastSeenUtc = lastSeen;
    }
#pragma warning disable CS8618
    private UserConnection() { }
#pragma warning restore

    public static UserConnection Create(IPAddress ip, DateTime timestamp)
    {
        ArgumentNullException.ThrowIfNull(ip);

        if (timestamp == default)
            throw new ArgumentException("Timestamp cannot be default.", nameof(timestamp));

        if (ip.Equals(IPAddress.None) || ip.Equals(IPAddress.Any))
            throw new ArgumentException("Invalid IP address.", nameof(ip));

        return new UserConnection(ip, timestamp);
    }

    public bool TryUpdateLastSeen(DateTime timestamp)
    {
        if (timestamp < LastSeenUtc) 
            return false;
        _lastSeenUtc = timestamp;
        return true;
    }

    #region IEquatable
    public bool Equals(UserConnection? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Ip.Equals(other.Ip);
    }

    public override bool Equals(object? obj)
        => Equals(obj as UserConnection);

    public override int GetHashCode()
        => Ip.GetHashCode();

    public static bool operator ==(UserConnection? left, UserConnection? right)
        => Equals(left, right);

    public static bool operator !=(UserConnection? left, UserConnection? right)
        => !Equals(left, right);
    #endregion
}
