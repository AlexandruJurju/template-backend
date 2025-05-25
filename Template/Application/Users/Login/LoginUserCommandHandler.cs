using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.Extensions.Configuration;

namespace Application.Users.Login;

internal sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    IConfiguration configuration,
    TimeProvider timeProvider
) : ICommandHandler<LoginUserCommand, LoginResponse>
{
    public async ValueTask<Result<LoginResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

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

        refreshTokenRepository.Add(refreshToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            tokenProvider.GenerateToken(user),
            refreshToken.Token
        );
    }
}
