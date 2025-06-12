using Application.Abstractions.Messaging;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;

namespace Application.Users.VerifyEmail;

public class VerifyEmailCommandHandler(
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider
) : ICommandHandler<VerifyEmailCommand>
{
    public async ValueTask<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        EmailVerificationToken? token = await emailVerificationTokenRepository
            .GetByIdAsync(request.TokenId, cancellationToken);

        if (token is null || token.ExpiresOnUtc < timeProvider.GetUtcNow().UtcDateTime)
        {
            return UserErrors.EmailVerificationTokenNotFound;
        }

        emailVerificationTokenRepository.Remove(token);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
