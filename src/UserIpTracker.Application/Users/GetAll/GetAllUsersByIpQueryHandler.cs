using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UserIpTracker.Application.Abstract;
using System.Data.Common;

namespace UserIpTracker.Application.Users.GetAll;

internal sealed class GetAllUsersByIpQueryHandler
    : IRequestHandler<GetAllUsersByIpQuery, List<UserConnectionDto>>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IIpNetworkBuilder _ipNetworkBuilder;
    private readonly ILogger<GetAllUsersByIpQueryHandler> _logger;

    public GetAllUsersByIpQueryHandler(
        IDbConnectionFactory connectionFactory,
        IIpNetworkBuilder ipNetworkBuilder,
        ILogger<GetAllUsersByIpQueryHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _ipNetworkBuilder = ipNetworkBuilder;
        _logger = logger;
    }

    public async Task<List<UserConnectionDto>> Handle(
        GetAllUsersByIpQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "Starting GetAllUsersByIpQuery for IP={Ip}",
            request.IpKeyword);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            if (!_ipNetworkBuilder.TryBuild(request.IpKeyword, out var network))
            {
                _logger.LogWarning(
                    "Invalid IP keyword '{IpKeyword}'",
                    request.IpKeyword);

                return [];
            }

            using var connection = _connectionFactory.GetConnection();

            var query = """
                SELECT DISTINCT
                    u.id AS UserId,
                    c.ip AS Ip
                    c.last_seen_utc AS LastSeenUtc
                FROM user_connections c
                    JOIN users u ON c.user_id = u.id
                WHERE c.ip <<= @Network::inet
                """;

            var command = new CommandDefinition(
                query,
                new { Network = network },
                cancellationToken: cancellationToken);

            var result = (await connection
                .QueryAsync<UserConnectionDto>(command)
                .ConfigureAwait(false))
                .ToList();

            stopwatch.Stop();

            _logger.LogInformation(
                "GetAllUsersByIpQuery completed successfully for IP={Ip}. " +
                "UserCount={Count}. ElapsedMs={ElapsedMs}",
                request.IpKeyword,
                result.Count,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                "GetAllUsersByIpQuery was cancelled for IP={Ip} after {ElapsedMs}ms",
                request.IpKeyword,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        catch (DbException ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Database error while GetAllUsersByIpQuery for IP={Ip} after {ElapsedMs}ms",
                request.IpKeyword,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "GetAllConnectionsQuery failed for IP={Ip} after {ElapsedMs}ms",
                request.IpKeyword,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
