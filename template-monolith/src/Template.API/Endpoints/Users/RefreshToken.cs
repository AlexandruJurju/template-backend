using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Template.Application.Users.Commands.Login;
using Template.Application.Users.Commands.RefreshToken;

namespace Template.API.Endpoints.Users;

/// <summary>
///     Refresh the JWT Token of a user
/// </summary>
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
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Refresh Token",
                Description = "Use the refresh token to get a new access token"
            })
            .Produces<LoginResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private sealed record RefreshTokenRequest(string RefreshToken);
}
