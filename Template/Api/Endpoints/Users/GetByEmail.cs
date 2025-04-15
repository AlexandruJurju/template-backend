using Api.ExceptionHandler;
using Application.Users.GetByEmail;
using Domain.Abstractions.Result;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Endpoints.Users;

internal sealed class GetByEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{email}", async (
                [FromRoute] string email,
                IMessageBus messageBus, CancellationToken cancellationToken) =>
            {
                Result<UserResponse> result = await messageBus.InvokeAsync<Result<UserResponse>>(new GetUserByEmailQuery(email), cancellationToken);

                return result.Match(
                    Results.Ok,
                    CustomResults.Problem
                );
            })
            .WithName("GetByEmail")
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<UserResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
