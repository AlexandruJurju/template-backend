using Domain.Abstractions.Persistence;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class EmailVerificationTokenRepository(
    ApplicationDbContext dbContext
) : IEmailVerificationTokenRepository
{
    public void Add(EmailVerificationToken emailVerificationToken)
    {
        dbContext.EmailVerificationTokens.Add(emailVerificationToken);
    }

    public async Task<EmailVerificationToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.EmailVerificationTokens
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public void Remove(EmailVerificationToken emailVerificationToken)
    {
        dbContext.EmailVerificationTokens.Remove(emailVerificationToken);
    }
}
