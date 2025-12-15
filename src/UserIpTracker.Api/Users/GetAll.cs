using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Endpoint;
using System.Data.Common;
using UserIpTracker.Application.Connections;
using UserIpTracker.Application.Connections.GetAll;
using UserIpTracker.Application.Users.GetAll;

namespace UserIpTracker.Api.Users;

public sealed class GetAll
    : IEndpoint<IResult, GetAllUsersByIpQuery>
{
    private readonly ISender _mediatr;

    public GetAll(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/search",
            ([FromQuery(Name = "ip")] string ip) => HandleAsync(new GetAllUsersByIpQuery(ip)))
            .Produces<List<UserConnectionDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("Find users by the initial or full part of their IP address")
            .WithDescription("Find users by the initial or full part of their IP address")
            .WithTags("UserEndpoints");
    }

    public async Task<IResult> HandleAsync(GetAllUsersByIpQuery request)
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
