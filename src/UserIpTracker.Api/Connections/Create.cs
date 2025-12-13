using MediatR;
using MinimalApi.Endpoint;
using SharedKernel.Domain;
using UserIpTracker.Application.Connections;
using UserIpTracker.Application.Connections.Create;

namespace UserIpTracker.Api.Connections;

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
        app.MapPost("/api/connections", HandleAsync)
            .Produces<UserConnectionDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("CreateConnection")
            .WithTags("Connection Endpoints");
    }

    public async Task<IResult> HandleAsync(CreateConnectionCommand request)
    {
        try
        {
            var result = await _mediatr.Send(request).ConfigureAwait(false);
            var location = $"/api/users/{result.UserId}/connections/{result.Ip}";
            return Results.Created(location, result);
        }
        catch (ArgumentException ex)
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
        catch (Exception)
        {
            return Results.Problem(
                title: "Unexpected error",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
