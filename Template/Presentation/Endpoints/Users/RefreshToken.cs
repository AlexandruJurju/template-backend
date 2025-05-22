using Application.Users.Login;
using Application.Users.RefreshToken;
using Domain.Abstractions.Result;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Presentation.ExceptionHandler;

namespace Presentation.Endpoints.Users;

/// <summary>
///     Refresh the JWT Token of a user
/// </summary>
internal sealed class RefreshToken : IEndpoint
{
    private sealed record RefreshTokenRequest(string RefreshToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/refresh-token", async (
                [FromBody] RefreshTokenRequest refreshToken,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new RefreshTokenCommand(refreshToken.RefreshToken);

                Result<RefreshTokenResponse> result = await sender.Send(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
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
}
