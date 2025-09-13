using Template.Application.Features.Users.Commands.Login;
using Template.Application.Features.Users.Commands.Register;
using Template.Application.Features.Users.Queries;
using Template.Application.Features.Users.Queries.GetAll;
using Template.Application.Features.Users.Queries.GetByEmail;
using Template.Application.Features.Users.Queries.GetById;
using Template.Common.SharedKernel.Api.Endpoints;
using Template.Common.SharedKernel.Infrastructure.Authentication.Jwt;
using Template.Domain.Entities.Users;

namespace Template.API.Endpoints.Users;

public static class UsersModule
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("users", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetAllUsersQuery();

                Result<IEnumerable<UserResponse>> result = await sender.Send(query, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithTags(Tags.Users)
            .WithName("GetAllUsers")
            .ProducesGet<IEnumerable<UserResponse>>()
            // .HasPermission(Permissions.UsersRead)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        app.MapGet("users/{email}", async (
                [FromRoute] string email,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetUserByEmailQuery(email);

                Result<UserResponse> result = await sender.Send(query, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithTags(Tags.Users)
            .WithName("GetUserByEmail")
            .WithOpenApi()
            .ProducesGet<UserResponse>(false, true);

        app.MapPost("users/register", async (
                [FromBody] RegisterRequest registerRequest,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new RegisterUserCommand(
                    registerRequest.Email,
                    registerRequest.FirstName,
                    registerRequest.LastName,
                    registerRequest.Password);

                Result<Guid> result = await sender.Send(command, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<User>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapGet("users/{userId:guid}", async (
                [FromRoute] Guid userId,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetUserByIdQuery(userId);

                Result<UserResponse> result = await sender.Send(query, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithTags(Tags.Users)
            .RequireAuthorization()
            .Produces<UserResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();

        app.MapPost("users/login", async (
                [FromBody] LoginRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new LoginUserCommand(
                    request.Email,
                    request.Password);

                Result<LoginResponse> result = await sender.Send(command, cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<LoginResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapGet("users/me", async (
                ISender sender, IUserContext userContext, CancellationToken cancellationToken) =>
            {
                Guid userId = userContext.GetUserId();

                Result<UserResponse> result = await sender.Send(new GetUserByIdQuery(userId), cancellationToken);

                return result.ToMinimalApiResult();
            })
            .WithTags(Tags.Users)
            .WithOpenApi()
            .Produces<UserResponse>();
    }
}
