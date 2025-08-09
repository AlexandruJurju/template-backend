using MediatR;
using Template.API.ExceptionHandler;
using Template.Application.Abstractions.Authentication;
using Template.Application.Users.Queries.GetById;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

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
            .WithName("Me")
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<UserResponse>();
    }
}
