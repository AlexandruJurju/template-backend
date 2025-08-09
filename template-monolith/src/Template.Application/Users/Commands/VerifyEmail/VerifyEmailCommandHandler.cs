using Microsoft.EntityFrameworkCore;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Users;
using Ardalis.Result;
using Template.SharedKernel.Application.Messaging;

namespace Template.Application.Users.Commands.VerifyEmail;

public class VerifyEmailCommandHandler(
    IApplicationDbContext dbContext,
    TimeProvider timeProvider
) : ICommandHandler<VerifyEmailCommand>
{
    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        EmailVerificationToken? token = await dbContext.EmailVerificationTokens
            .SingleOrDefaultAsync(x => x.Id == request.TokenId, cancellationToken);

        if (token is null || token.ExpiresOnUtc < timeProvider.GetUtcNow().UtcDateTime)
        {
            return UserErrors.EmailVerificationTokenNotFound();
        }

        dbContext.EmailVerificationTokens.Remove(token);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
