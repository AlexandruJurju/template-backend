using MediatR;
using Microsoft.AspNetCore.Mvc;
using Template.API.ExceptionHandler;
using Template.Application.Users.Queries.GetById;
using Template.Domain.Abstractions.Result;

namespace Template.API.Endpoints.Users;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{userId:guid}", async (
                [FromRoute] Guid userId,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetUserByIdQuery(userId);

                Result<UserResponse> result = await sender.Send(query, cancellationToken);

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
