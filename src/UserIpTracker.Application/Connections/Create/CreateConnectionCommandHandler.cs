using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Domain;
using System.Net;
using UserIpTracker.Domain;

namespace UserIpTracker.Application.Connections.Create;

internal sealed class CreateConnectionCommandHandler
    : IRequestHandler<CreateConnectionCommand, UserConnectionDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateConnectionCommandHandler> _logger;

    public CreateConnectionCommandHandler(
        IUserRepository userRepository,
        ILogger<CreateConnectionCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserConnectionDto?> Handle(
        CreateConnectionCommand request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
             "Registering request. UserId={UserId}, Ip={Ip}, Timestamp={Timestamp}",
             request.UserId,
             request.Ip,
             request.TimestampUtc);

        try
        {
            if (!IPAddress.TryParse(request.Ip, out var ip))
            {
                _logger.LogError(
                    "Invalid IP address received. UserId={UserId}, Ip={Ip}",
                    request.UserId,
                    request.Ip);

                return null;
            }

            var timestamp = request.TimestampUtc ?? DateTime.UtcNow;

            var userConnection = UserConnection.Create(ip, timestamp);
            var userId = new UserId(request.UserId);

            var user = await _userRepository
                .GetByIdWithConnectionsAsync(userId, cancellationToken)
                .ConfigureAwait(false);

            if (user is null)
            {
                _logger.LogInformation(
                    "User not found. Creating new user. UserId={UserId}",
                    userId);

                user = User.Create(userId, userConnection);

                await _userRepository
                    .CreateAsync(user, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                user.RegisterConnection(userConnection);
            }

            await _userRepository
                .StoreAsync(cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation(
                "Connection registered successfully. UserId={UserId}, Ip={Ip}, Timestamp={Timestamp}",
                userId,
                ip,
                timestamp);

            return new UserConnectionDto(user.Id.Key, request.Ip, timestamp);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "CreateConnection operation cancelled. UserId={UserId}",
                request.UserId);

            throw;
        }
        catch (PersistenceException ex)
        {
            _logger.LogError(
                ex,
                "Database error while registering request. UserId={UserId}, Ip={Ip}",
                request.UserId,
                request.Ip);

            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(
                ex,
                "Domain validation error while registering request. UserId={UserId}, Ip={Ip}",
                request.UserId,
                request.Ip);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(
                ex,
                "Unexpected error while registering request. UserId={UserId}, Ip={Ip}",
                request.UserId,
                request.Ip);

            throw;
        }
    }
}
