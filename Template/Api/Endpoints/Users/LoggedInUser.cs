using Api.ExceptionHandler;
using Application.Users.GetById;
using Application.Users.LoggedInUser;
using Domain.Abstractions.Result;
using Wolverine;

namespace Api.Endpoints.Users;

public class LoggedInUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/me", async (
                IMessageBus messageBus, CancellationToken cancellationToken) =>
            {
                var result = await messageBus.InvokeAsync<Result<UserResponse>>(new LoggedInUserQuery(), cancellationToken);

                return result.Match(
                    Results.Ok,
                    CustomResults.Problem
                );
            })
            .WithName("LoggedInUser")
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<UserResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}