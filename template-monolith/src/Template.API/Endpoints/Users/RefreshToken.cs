using Template.Application.Users.Commands.Login;
using Template.Application.Users.Commands.RefreshToken;

namespace Template.API.Endpoints.Users;

internal sealed class RefreshToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/refresh-token", async (
                [FromBody] RefreshTokenRequest refreshToken,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new RefreshTokenCommand(refreshToken.RefreshToken);

                Result<RefreshTokenResponse> result = await sender.Send(command, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithName("RefreshToken")
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<LoginResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private sealed record RefreshTokenRequest(string RefreshToken);
}
