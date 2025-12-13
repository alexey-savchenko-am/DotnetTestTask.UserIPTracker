using SharedKernel.Domain;
using System.Net;

namespace UserIpTracker.Domain;

public sealed class User
    : IRoot<UserId>
{
    private readonly List<UserConnection> _connections = [];
    private Dictionary<IPAddress, UserConnection>? _connectionDictionary;
    private Dictionary<IPAddress, UserConnection> ConnectionsDictionary
    {
        get
        {
            return _connectionDictionary ??=
                _connections.ToDictionary(c => c.Ip);
        }
    }

    public UserId Id { get; }
    public IReadOnlyCollection<UserConnection> Connections => _connections.AsReadOnly();
    public UserConnection? LastConnection { get; private set; }

#pragma warning disable CS8618
    private User() { }
#pragma warning restore

    private User(UserId? userId) 
    { 
        Id = userId ?? UserId.Create();
    }

    public static User Create(UserId? userId, params UserConnection[] connections)
    {
        var user = new User(userId);

        if(connections is null || connections.Length == 0)
            return user;

        foreach(var connection in connections)
        {
            user.RegisterConnection(connection);
        }

        return user;
    }
   
    public void RegisterConnection(UserConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (ConnectionsDictionary.TryGetValue(connection.Ip, out var existed))
        {
            existed.TryUpdateLastSeen(connection.LastSeenUtc);
        }
        else
        {
            _connections.Add(connection);
            ConnectionsDictionary[connection.Ip] = connection;
        }

        if (LastConnection is null || connection.LastSeenUtc > LastConnection.LastSeenUtc)
        {
            LastConnection = UserConnection.Create(connection.Ip, connection.LastSeenUtc);
        }
    }
}