using Microsoft.EntityFrameworkCore;
using Template.Application.Abstractions.Authentication;
using Template.Application.Abstractions.Messaging;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Abstractions.Result;
using Template.Domain.Users;

namespace Template.Application.Users.RefreshToken;

public class RefreshTokenCommandHandler(
    IApplicationDbContext dbContext,
    TimeProvider timeProvider,
    ITokenProvider tokenProvider
) : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async ValueTask<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Domain.Users.RefreshToken? refreshToken = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (refreshToken is null || refreshToken.ExpiresOnUtc < timeProvider.GetUtcNow().UtcDateTime)
        {
            return UserErrors.RefreshTokenExpired;
        }

        string accessToken = tokenProvider.GenerateToken(refreshToken.User);

        refreshToken.Token = tokenProvider.GenerateRefreshToken();
        refreshToken.ExpiresOnUtc = timeProvider.GetUtcNow().UtcDateTime.AddDays(7);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(accessToken, refreshToken.Token);
    }
}
