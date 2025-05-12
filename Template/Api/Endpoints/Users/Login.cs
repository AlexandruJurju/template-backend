using Api.ExceptionHandler;
using Application.Users.Login;
using Domain.Abstractions.Result;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.Users;

/// <summary>
///     Authenticates a user and returns a JWT token
/// </summary>
internal sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login", async (
                [FromBody] Request request,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new LoginUserCommand(
                    request.Email,
                    request.Password);

                Result<string> result = await sender.Send(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithName("LoginUser")
            .WithTags(Tags.Users)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Authenticate user",
                Description = "Authenticates user credentials and returns JWT token"
            })
            .Produces<string>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }


    /// <summary>
    ///     Login request payload
    /// </summary>
    /// <param name="Email">User's email address</param>
    /// <param name="Password">User's password</param>
    private sealed record Request(string Email, string Password);
}
