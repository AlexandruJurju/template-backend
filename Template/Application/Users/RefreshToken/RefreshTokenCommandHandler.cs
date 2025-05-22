using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.RefreshToken;

public class RefreshTokenCommandHandler(
    IApplicationDbContext dbContext,
    ITokenProvider tokenProvider
) : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async ValueTask<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Domain.Users.RefreshToken? refreshToken = await dbContext.RefreshTokens
            .Include(e => e.User)
            .ThenInclude(e => e.Role)
            .ThenInclude(e => e.Permissions)
            .SingleOrDefaultAsync(e => e.Token == request.RefreshToken, cancellationToken);

        if (refreshToken is null || refreshToken.ExpiresOnUtc < TimeProvider.System.GetUtcNow().UtcDateTime)
        {
            return UserErrors.RefreshTokenExpired;
        }

        string accessToken = tokenProvider.GenerateToken(refreshToken.User);

        refreshToken.Token = tokenProvider.GenerateRefreshToken();
        refreshToken.ExpiresOnUtc = TimeProvider.System.GetUtcNow().UtcDateTime.AddDays(7);

        return new RefreshTokenResponse(accessToken, refreshToken.Token);
    }
}
