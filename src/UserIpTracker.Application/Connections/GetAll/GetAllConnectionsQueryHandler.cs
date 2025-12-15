using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UserIpTracker.Application.Abstract;
using UserIpTracker.Application.Connections.GetAll;
using UserIpTracker.Application.Connections;
using SharedKernel.Domain;
using System.Data.Common;

internal sealed class GetAllConnectionQueryHandler
    : IRequestHandler<GetAllConnectionsQuery, List<UserConnectionDto>>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<GetLastConnectionQueryHandler> _logger;

    public GetAllConnectionQueryHandler(
        IDbConnectionFactory connectionFactory,
        ILogger<GetLastConnectionQueryHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<List<UserConnectionDto>> Handle(
        GetAllConnectionsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "Starting GetAllConnectionsQuery for UserId={UserId}",
            request.UserId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var connection = _connectionFactory.GetConnection();

            var query = """
                SELECT 
                    u.id AS UserId,
                    c.ip::text AS Ip,
                    c.last_seen_utc AS LastSeenUtc
                FROM users u
                    JOIN user_connections c ON u.id = c.user_id
                WHERE u.id = @UserId
                """;

            var command = new CommandDefinition(
                query,
                new { request.UserId },
                cancellationToken: cancellationToken);

            var result = (await connection
                .QueryAsync<UserConnectionDto>(command)
                .ConfigureAwait(false))
                .ToList();

            stopwatch.Stop();

            _logger.LogInformation(
                "GetAllConnectionsQuery completed successfully for UserId={UserId}. " +
                "ConnectionsCount={Count}. ElapsedMs={ElapsedMs}",
                request.UserId,
                result.Count,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                "GetAllConnectionsQuery was cancelled for UserId={UserId} after {ElapsedMs}ms",
                request.UserId,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        catch (DbException ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Database error while GetAllConnectionsQuery for UserId={UserId} after {ElapsedMs}ms",
                request.UserId,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "GetAllConnectionsQuery failed for UserId={UserId} after {ElapsedMs}ms",
                request.UserId,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
