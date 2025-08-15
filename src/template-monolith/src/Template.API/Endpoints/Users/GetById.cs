using Template.Application.Features.Users;
using Template.Application.Features.Users.Queries.GetById;

namespace Template.API.Endpoints.Users;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{userId:guid}", async (
                [FromRoute] Guid userId,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetUserByIdQuery(userId);

                Result<UserResponse> result = await sender.Send(query, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithTags(Tags.Users)
            .RequireAuthorization()
            .Produces<UserResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }
}
