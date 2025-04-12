using Api.ExceptionHandler;
using Application.Users.Login;
using Domain.Abstractions.Result;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Endpoints.Users;

public class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login", async (
                [FromBody] Request request,
                IMessageBus messageBus, CancellationToken cancellationToken) =>
            {
                var command = new LoginUserCommand(
                    request.Email,
                    request.Password);

                var result = await messageBus.InvokeAsync<Result<string>>(command, cancellationToken);

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