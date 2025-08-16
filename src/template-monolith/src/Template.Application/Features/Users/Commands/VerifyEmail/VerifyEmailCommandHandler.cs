using Template.Common.SharedKernel.Application.CQRS.Commands;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Commands.VerifyEmail;

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
