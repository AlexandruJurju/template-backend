using MediatR;
using Template.API.ExceptionHandler;
using Template.Application.Users.Commands.VerifyEmail;
using Template.SharedKernel.Application.CustomResult;

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

                return result.IsSuccess
                    ? Results.Ok()
                    : CustomResults.Problem(result);
            })
            .WithName("VerifyEmail")
            .WithTags(Tags.Users)
            .WithOpenApi();
    }
}
