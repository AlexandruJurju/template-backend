using Api.ExceptionHandler;
using Application.Users.GetAll;
using Domain.Abstractions.Result;
using Wolverine;

namespace Api.Endpoints.Users;

public class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users", async (
                IMessageBus messageBus, CancellationToken cancellationToken) =>
            {
                var result = await messageBus.InvokeAsync<Result<List<UserResponse>>>(new GetAllUsersQuery(), cancellationToken);

                return result.Match(
                    Results.Ok,
                    CustomResults.Problem
                );
            })
            .WithName("GetAll")
            .WithTags(Tags.Users)
            .WithOpenApi()
            .RequireAuthorization(Roles.Member)
            .Produces<List<UserResponse>>();
    }
}