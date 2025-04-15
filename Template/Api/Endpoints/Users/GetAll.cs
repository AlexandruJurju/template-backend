using Api.ExceptionHandler;
using Api.Extensions;
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
                Result<List<UserResponse>> result = await messageBus.InvokeAsync<Result<List<UserResponse>>>(new GetAllUsersQuery(), cancellationToken);

                return result.Match(
                    Results.Ok,
                    CustomResults.Problem
                );
            })
            .WithName("GetAll")
            .WithTags(Tags.Users)
            .WithOpenApi()
            .HasPermission(Permissions.UsersRead)
            .Produces<List<UserResponse>>();
    }
}
