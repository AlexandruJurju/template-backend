using Microsoft.AspNetCore.Mvc;
using Template.API.Authentication;
using Template.Application.Users.Queries.GetByEmail;

namespace Template.API.Endpoints.Users;

internal sealed class GetByEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{email}", async (
                [FromRoute] string email,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetUserByEmailQuery(email);

                Result<UserResponse> result = await sender.Send(query, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithName("GetByEmail")
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<UserResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ApiKeyEndpointFilter>();
    }
}
