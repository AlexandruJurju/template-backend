using Template.Application.Features.Users.Commands.Register;
using Template.Domain.Entities.Users;

namespace Template.API.Endpoints.Users;

internal sealed class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (
                [FromBody] Request request,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new RegisterUserCommand(
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.Password);

                Result<Guid> result = await sender.Send(command, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithName("RegisterUser")
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<User>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private sealed record Request(string Email, string FirstName, string LastName, string Password);
}
