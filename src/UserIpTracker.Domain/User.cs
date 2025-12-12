using SharedKernel.Domain;
using System.Net;

namespace UserIpTracker.Domain;

public sealed class User
    : IRoot<UserId>
{
    private readonly Dictionary<IPAddress, UserConnection> _connections = new();

    public UserId Id { get; }
    public IReadOnlyCollection<UserConnection> Connections => _connections.Values;
    public UserConnection? LastConnection { get; private set; }

    private User() 
    { 
        Id = UserId.Create(); 
    }

    public static User Create()
    {
        return new User();
    }

    public void RegisterConnection(IPAddress ip, DateTime timestamp)
    {
        if (_connections.TryGetValue(ip, out var conn))
        {
            conn.Update(timestamp);
        }
        else
        {
            _connections[ip] = UserConnection.Create(ip, timestamp);
        }

        if (LastConnection is null || timestamp > LastConnection.LastSeen)
        {
            LastConnection = UserConnection.Create(ip, timestamp);
        }
    }
}