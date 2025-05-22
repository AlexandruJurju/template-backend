using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Login;

internal sealed class LoginUserCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider
) : ICommandHandler<LoginUserCommand, LoginResponse>
{
    public async ValueTask<Result<LoginResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .Include(user => user.Role)
            .ThenInclude(role => role.Permissions)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound(command.Email);
        }

        bool verified = passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verified)
        {
            return UserErrors.NotFound(command.Email);
        }

        // todo: use config
        var refreshToken = new Domain.Users.RefreshToken
        {
            Id = Guid.NewGuid(),
            ExpiresOnUtc = TimeProvider.System.GetUtcNow().UtcDateTime.AddDays(7),
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
