using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UserIpTracker.Application.Abstract;
using UserIpTracker.Application.Connections;
using System.Data.Common;

using UserIpTracker.Application.Connections.GetLast;

internal sealed class GetLastConnectionByIpQueryHandler
    : IRequestHandler<GetLastConnectionByIpQuery, UserConnectionDto?>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<GetLastConnectionByIpQueryHandler> _logger;

    public GetLastConnectionByIpQueryHandler(
        IDbConnectionFactory connectionFactory,
        ILogger<GetLastConnectionByIpQueryHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<UserConnectionDto?> Handle(
        GetLastConnectionByIpQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "Starting GetLastConnectionByIpQuery for UserId={UserId}, IP = {Ip}",
            request.UserId, 
            request.Ip);

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
                WHERE u.id = @UserId AND c.ip = @Ip::inet
                """;

            var command = new CommandDefinition(
                query,
                new { request.UserId, request.Ip },
                cancellationToken: cancellationToken);

            var result = await connection
                .QuerySingleOrDefaultAsync<UserConnectionDto>(command)
                .ConfigureAwait(false);

            stopwatch.Stop();

            _logger.LogInformation(
                "GetLastConnectionByIpQuery completed successfully for UserId={UserId}, IP={Ip}. " +
                "ElapsedMs={ElapsedMs}",
                request.UserId,
                request.Ip,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                "GetLastConnectionByIpQuery was cancelled for UserId={UserId}, IP={Ip} after {ElapsedMs}ms",
                request.UserId,
                request.Ip,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        catch (DbException ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Database error while GetLastConnectionByIpQuery for UserId={UserId}, IP={Ip} after {ElapsedMs}ms",
                request.UserId,
                request.Ip,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "GetLastConnectionByIpQuery failed for UserId={UserId}, IP={Ip} after {ElapsedMs}ms",
                request.UserId,
                request.Ip,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
