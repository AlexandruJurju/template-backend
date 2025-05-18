using Presentation.Extensions;
using Application.Users.GetAll;
using Domain.Abstractions.Result;
using Mediator;
using Presentation.ExceptionHandler;

namespace Presentation.Endpoints.Users;

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
            .HasPermission(Permissions.UsersRead)
            .Produces<IEnumerable<UserResponse>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}
