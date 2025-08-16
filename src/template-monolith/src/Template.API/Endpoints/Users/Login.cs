using Microsoft.AspNetCore.Mvc;
using Template.Application.Features.Users.Commands.Login;
using Template.Common.SharedKernel.Api.Endpoints;

namespace Template.API.Endpoints.Users;

internal sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login", async (
                [FromBody] Request request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new LoginUserCommand(
                    request.Email,
                    request.Password);

                Result<LoginResponse> result = await sender.Send(command, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<LoginResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private sealed record Request(string Email, string Password);
}
