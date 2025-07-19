using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Template.Application.Abstractions.Authentication;
using Template.Application.Abstractions.Messaging;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Abstractions.Result;
using Template.Domain.Users;

namespace Template.Application.Users.Commands.Login;

internal sealed class LoginUserCommandHandler(
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    IConfiguration configuration,
    TimeProvider timeProvider,
    IApplicationDbContext dbContext
) : ICommandHandler<LoginUserCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .Include(u => u.Role)
            .ThenInclude(u => u.Permissions)
            .SingleOrDefaultAsync(user => user.Email == command.Email, cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound(command.Email);
        }

        bool verified = passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verified)
        {
            return UserErrors.NotFound(command.Email);
        }

        var refreshToken = new Domain.Users.RefreshToken
        {
            Id = Guid.NewGuid(),
            ExpiresOnUtc = timeProvider.GetUtcNow().UtcDateTime.AddMinutes(configuration.GetValue<int>("Jwt:RefreshTokenExpireInMinutes")),
            Token = tokenProvider.GenerateRefreshToken(),
            UserId = user.Id
        };

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            tokenProvider.GenerateToken(user),
            refreshToken.Token
        );
    }
}
