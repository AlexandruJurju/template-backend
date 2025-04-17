using Api.ExceptionHandler;
using Api.Extensions;
using Application.Users.GetAll;
using Domain.Abstractions.Result;
using MediatR;

namespace Api.Endpoints.Users;

public class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users", async (
                ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetAllUsersQuery();

                Result<IEnumerable<UserResponse>> result = await sender.Send(query, cancellationToken);

                return result.Match(
                    Results.Ok,
                    CustomResults.Problem
                );
            })
            .WithName("GetAll")
            .WithTags(Tags.Users)
            .Produces<IEnumerable<UserResponse>>()
            .WithOpenApi()
            .HasPermission(Permissions.UsersRead);
    }
}
