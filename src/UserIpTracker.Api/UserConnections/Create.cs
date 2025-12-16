using MediatR;
using MinimalApi.Endpoint;
using SharedKernel.Domain.Exceptions;
using UserIpTracker.Application;
using UserIpTracker.Application.Connections.Create;

namespace UserIpTracker.Api.UserConnections;

public sealed class Create
    : IEndpoint<IResult, CreateConnectionCommand>
{
    private readonly ISender _mediatr;

    public Create(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/connections", HandleAsync)
            .Produces<UserConnectionDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("Create a new connection of a user. If the user does not exist, a new one will be created.")
            .WithDescription("Create a new connection of a user. If the user does not exist, a new one will be created.")
            .WithTags("UserConnectionsEndpoints");
    }

    public async Task<IResult> HandleAsync(CreateConnectionCommand request)
    {
        try
        {
            var result = await _mediatr.Send(request).ConfigureAwait(false);
            var location = $"/api/users/{result.UserId}/connections";
            return Results.Created(location, result);
        }
        catch (ArgumentException ex)
        {
            return Results.Problem(
                title: "Invalid request",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (ApplicationValidationException ex)
        {
            return Results.Problem(
                title: "Invalid request",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (PersistenceException ex)
        {
            return Results.Problem(
                title: "Persistence error",
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict);
        }
        catch (OperationCanceledException)
        {
            return Results.StatusCode(StatusCodes.Status499ClientClosedRequest);
        }
    }
}
