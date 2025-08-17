using Template.Application.Features.Users;
using Template.Application.Features.Users.Dto;
using Template.Application.Features.Users.Queries.GetAll;
using Template.Common.SharedKernel.Api.Endpoints;

namespace Template.API.Endpoints.Users;

public class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetAllUsersQuery();

                Result<IEnumerable<UserResponse>> result = await sender.Send(query, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithTags(Tags.Users)
            .WithName(nameof(GetAll))
            .ProducesGet<IEnumerable<UserResponse>>()
            // .HasPermission(Permissions.UsersRead)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}
