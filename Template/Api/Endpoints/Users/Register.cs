using Api.ExceptionHandler;
using Application.Users.Register;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Endpoints.Users;

internal sealed class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (
                [FromBody] Request request,
                IMessageBus messageBus, CancellationToken cancellationToken) =>
            {
                var command = new RegisterUserCommand(
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.Password);

                Result<Guid> result = await messageBus.InvokeAsync<Result<Guid>>(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithName("RegisterUser")
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<User>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private sealed record Request(string Email, string FirstName, string LastName, string Password);
}
