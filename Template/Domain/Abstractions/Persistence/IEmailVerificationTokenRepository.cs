using Domain.Users;

namespace Domain.Abstractions.Persistence;

public interface IEmailVerificationTokenRepository
{
    void Add(EmailVerificationToken emailVerificationToken);
    Task<EmailVerificationToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Remove(EmailVerificationToken emailVerificationToken);
}
