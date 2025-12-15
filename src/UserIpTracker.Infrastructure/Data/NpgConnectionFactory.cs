using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using UserIpTracker.Application.Abstract;

namespace UserIpTracker.Infrastructure.Data;

public sealed class NpgConnectionFactory
    : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _connectionString = options?.Value.ConnectionString
            ?? throw new ArgumentNullException(nameof(options));
    }

    public IDbConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}