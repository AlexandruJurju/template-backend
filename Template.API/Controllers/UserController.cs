using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.API.Authentication;
using Template.API.Extensions;
using Template.Application.Abstractions.Authentication;
using Template.Application.Users.Commands.Login;
using Template.Application.Users.Commands.RefreshToken;
using Template.Application.Users.Commands.Register;
using Template.Application.Users.Commands.VerifyEmail;
using Template.Application.Users.Queries.GetAll;
using Template.Application.Users.Queries.GetByEmail;
using Template.Application.Users.Queries.GetById;
using Template.Domain.Abstractions.Result;
using UserResponse = Template.Application.Users.Queries.GetAll.UserResponse;

namespace Template.API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(ISender sender, IUserContext userContext) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permissions.UsersRead)]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery();

        Result<IEnumerable<UserResponse>> result = await sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: value => Ok(value),
            onFailure: this.Problem
        );
    }

    [HttpGet("{email}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ServiceFilter(typeof(ApiKeyEndpointFilter))]
    public async Task<IActionResult> GetByEmail([FromRoute] string email, CancellationToken cancellationToken)
    {
        var query = new GetUserByEmailQuery(email);

        Result<Application.Users.Queries.GetByEmail.UserResponse> result = await sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: value => Ok(value),
            onFailure: this.Problem
        );
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(userId);

        Result<Application.Users.Queries.GetById.UserResponse> result = await sender.Send(query, cancellationToken);

        return result.Match(
            onSuccess: value => Ok(value),
            onFailure: this.Problem
        );
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Email, request.Password);

        Result<LoginResponse> result = await sender.Send(command, cancellationToken);

        return result.Match(
            onSuccess: value => Ok(value),
            onFailure: this.Problem
        );
    }


    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        Guid userId = userContext.GetUserId();

        Result<Application.Users.Queries.GetById.UserResponse> result = await sender.Send(new GetUserByIdQuery(userId), cancellationToken);

        return result.Match(
            onSuccess: value => Ok(value),
            onFailure: this.Problem
        );
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshToken, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(refreshToken.RefreshToken);
        Result<RefreshTokenResponse> result = await sender.Send(command, cancellationToken);
        return result.Match(
            onSuccess: value => Ok(value),
            onFailure: this.Problem
        );
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);

        Result<Guid> result = await sender.Send(command, cancellationToken);
        return result.Match(
            userId => CreatedAtAction(nameof(GetById), new { userId }, userId),
            this.Problem);
    }

    [HttpGet("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyEmail([FromQuery] Guid token, CancellationToken cancellationToken)
    {
        Result result = await sender.Send(new VerifyEmailCommand(token), cancellationToken);
        return result.IsSuccess ? Ok() : this.Problem(result);
    }

    public sealed record LoginRequest(string Email, string Password);

    public sealed record RefreshTokenRequest(string RefreshToken);

    public sealed record RegisterRequest(string Email, string FirstName, string LastName, string Password);
}
