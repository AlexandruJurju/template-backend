using Template.Application.Contracts.Authentication;
using Template.Application.Features.Users;
using Template.Application.Features.Users.Queries.GetById;
using Template.Common.SharedKernel.Api.Endpoints;

namespace Template.API.Endpoints.Users;

public class Me : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/me", async (
                ISender sender, IUserContext userContext, CancellationToken cancellationToken) =>
            {
                Guid userId = userContext.GetUserId();

                Result<UserResponse> result = await sender.Send(new GetUserByIdQuery(userId), cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<UserResponse>();
    }
}
