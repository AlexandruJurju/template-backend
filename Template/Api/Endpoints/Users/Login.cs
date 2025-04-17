using Api.ExceptionHandler;
using Application.Users.Login;
using Domain.Abstractions.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Users;

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
            .WithOpenApi()
            .Produces<string>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private sealed record Request(string Email, string Password);
}
