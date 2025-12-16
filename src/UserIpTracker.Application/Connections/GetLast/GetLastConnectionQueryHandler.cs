using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UserIpTracker.Application.Abstract;
using System.Data.Common;
using UserIpTracker.Application.Connections.GetLast;
using UserIpTracker.Application;

internal sealed class GetLastConnectionQueryHandler
    : IRequestHandler<GetLastConnectionQuery, UserConnectionDto?>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<GetLastConnectionQueryHandler> _logger;

    public GetLastConnectionQueryHandler(
        IDbConnectionFactory connectionFactory,
        ILogger<GetLastConnectionQueryHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<UserConnectionDto?> Handle(
        GetLastConnectionQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "Starting GetLastConnectionQuery for UserId={UserId}",
            request.UserId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var connection = _connectionFactory.GetConnection();

            var query = """
                SELECT 
                    u.id AS UserId,
                    u.last_ip::text AS Ip,
                    u.last_seen_utc AS LastSeenUtc
                FROM users u
                WHERE u.id = @UserId
                """;

            var command = new CommandDefinition(
                query,
                new { request.UserId },
                cancellationToken: cancellationToken);

            var result = await connection
                .QuerySingleOrDefaultAsync<UserConnectionDto>(command)
                .ConfigureAwait(false);

            stopwatch.Stop();

            _logger.LogInformation(
                "GetLastConnectionQuery completed successfully for UserId={UserId}. " +
                "ElapsedMs={ElapsedMs}",
                request.UserId,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                "GetLastConnectionQuery was cancelled for UserId={UserId} after {ElapsedMs}ms",
                request.UserId,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        catch (DbException ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Database error while GetLastConnectionQuery for UserId={UserId} after {ElapsedMs}ms",
                request.UserId,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "GetLastConnectionQuery failed for UserId={UserId} after {ElapsedMs}ms",
                request.UserId,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
