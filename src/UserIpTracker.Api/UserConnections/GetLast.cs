using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Endpoint;
using System.Data.Common;
using UserIpTracker.Application;
using UserIpTracker.Application.Connections.GetLast;

namespace UserIpTracker.Api.UserConnections;

public sealed class GetLast
    : IEndpoint<IResult, GetLastConnectionQuery>
{
    private readonly ISender _mediatr;

    public GetLast(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{userId:guid}/connections/last",
            ([FromRoute] Guid userId) => HandleAsync(new GetLastConnectionQuery(userId)))
            .Produces<UserConnectionDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("Find the time and IP address of the user's last connection")
            .WithDescription("Find the time and IP address of the user's last connection")
            .WithTags("UserConnectionsEndpoints");
    }

    public async Task<IResult> HandleAsync(GetLastConnectionQuery request)
    {
        try
        {
            var result = await _mediatr.Send(request).ConfigureAwait(false);

            if(result is null)
            {
                return Results.NotFound();
            }    

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
