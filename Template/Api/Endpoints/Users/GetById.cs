using Api.ExceptionHandler;
using Application.Users.GetById;
using Domain.Abstractions.Result;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Endpoints.Users;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{userId:guid}", async (
                [FromRoute] Guid userId,
                IMessageBus messageBus, CancellationToken cancellationToken) =>
            {
                var result = await messageBus.InvokeAsync<Result<UserResponse>>(new GetUserByIdQuery(userId), cancellationToken);

                return result.Match(
                    Results.Ok,
                    CustomResults.Problem
                );
            })
            .WithName("GetById")
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<UserResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}