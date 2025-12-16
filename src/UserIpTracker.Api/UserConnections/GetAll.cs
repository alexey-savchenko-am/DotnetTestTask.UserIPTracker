using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Endpoint;
using System.Data.Common;
using UserIpTracker.Application;
using UserIpTracker.Application.Connections.GetAll;

namespace UserIpTracker.Api.UserConnections;

public sealed class GetAll
    : IEndpoint<IResult, GetAllConnectionsQuery>
{
    private readonly ISender _mediatr;

    public GetAll(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{userId:guid}/connections",
            ([FromRoute] Guid userId) => HandleAsync(new GetAllConnectionsQuery(userId)))
            .Produces<List<UserConnectionDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("Find all accumulated connections of a user")
            .WithDescription("Find all accumulated connections of a user")
            .WithTags("UserConnectionsEndpoints");
    }

    public async Task<IResult> HandleAsync(GetAllConnectionsQuery request)
    {
        try
        {
            var result = await _mediatr.Send(request).ConfigureAwait(false);
            return Results.Ok(result);
        }
        catch (DbException ex)
        {
            return Results.Problem(
                title: "Persistence error",
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict);
        }
        catch (OperationCanceledException)
        {
            return Results.Conflict(StatusCodes.Status499ClientClosedRequest);
        }
    }
}
