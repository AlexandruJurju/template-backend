using Template.Application.Features.Users;
using Template.Application.Features.Users.Dto;
using Template.Application.Features.Users.Queries.GetByEmail;
using Template.Common.SharedKernel.Api.Endpoints;

namespace Template.API.Endpoints.Users;

internal sealed class GetByEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{email}", async (
                [FromRoute] string email,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetUserByEmailQuery(email);

                Result<UserResponse> result = await sender.Send(query, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithTags(Tags.Users)
            .WithName(nameof(GetByEmail))
            .WithOpenApi()
            .ProducesGet<UserResponse>(false, true);
    }
}
