using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MediatR;
using Template.Application.Users.Commands.VerifyEmail;

namespace Template.API.Endpoints.Users;

public class VerifyLink : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/verify-email", async (
                Guid token,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(new VerifyEmailCommand(token), cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithName("VerifyEmail")
            .WithTags(Tags.Users)
            .WithOpenApi();
    }
}
