using Microsoft.EntityFrameworkCore;
using Template.Domain.ApiKeys;
using Template.Domain.EmailTemplates;
using Template.Domain.Infrastructure.Outbox;
using Template.Domain.Users;

namespace Template.Domain.Abstractions.Persistence;

public interface IApplicationDbContext
{
    DbSet<Role> Roles { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<OutboxMessage> OutboxMessages { get; set; }
    DbSet<ApiKey> ApiKeys { get; set; }
    DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    DbSet<EmailTemplate> EmailTemplates { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
