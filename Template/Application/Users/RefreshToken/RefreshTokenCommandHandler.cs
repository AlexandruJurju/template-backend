using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;

namespace Application.Users.RefreshToken;

public class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ITokenProvider tokenProvider
) : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async ValueTask<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Domain.Users.RefreshToken? refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (refreshToken is null || refreshToken.ExpiresOnUtc < timeProvider.GetUtcNow().UtcDateTime)
        {
            return UserErrors.RefreshTokenExpired;
        }

        string accessToken = tokenProvider.GenerateToken(refreshToken.User);

        refreshToken.Token = tokenProvider.GenerateRefreshToken();
        refreshToken.ExpiresOnUtc = timeProvider.GetUtcNow().UtcDateTime.AddDays(7);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(accessToken, refreshToken.Token);
    }
}
