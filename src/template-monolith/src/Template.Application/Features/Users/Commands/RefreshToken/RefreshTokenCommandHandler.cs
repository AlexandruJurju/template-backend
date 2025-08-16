using Template.Application.Contracts.Authentication;
using Template.Common.SharedKernel.Application.CQRS.Commands;
using Template.Common.SharedKernel.Infrastructure;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IApplicationDbContext dbContext,
    ITokenProvider tokenProvider
) : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Users.RefreshToken? refreshToken = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTimeHelper.UtcNow())
        {
            return UserErrors.RefreshTokenExpired;
        }

        string accessToken = tokenProvider.GenerateToken(refreshToken.User);

        refreshToken.Token = tokenProvider.GenerateRefreshToken();
        refreshToken.ExpiresOnUtc = DateTimeHelper.UtcNow().AddDays(7);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(accessToken, refreshToken.Token);
    }
}
