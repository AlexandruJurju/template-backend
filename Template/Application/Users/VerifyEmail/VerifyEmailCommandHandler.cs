using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.VerifyEmail;

public class VerifyEmailCommandHandler(
    IApplicationDbContext dbContext,
    TimeProvider timeProvider
) : ICommandHandler<VerifyEmailCommand>
{
    public async ValueTask<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        EmailVerificationToken? token = await dbContext.EmailVerificationTokens
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == request.TokenId, cancellationToken);

        if (token is null || token.ExpiresOnUtc < timeProvider.GetUtcNow().UtcDateTime)
        {
            return UserErrors.EmailVerificationTokenNotFound;
        }

        dbContext.EmailVerificationTokens.Remove(token);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
