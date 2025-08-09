using Template.Domain.ApiKeys;
using Template.Domain.EmailTemplates;
using Template.Domain.Users;
using Template.SharedKernel.Infrastructure.Outbox;

namespace Template.Domain.Abstractions.Persistence;

public interface IApplicationDbContext : IUnitOfWork
{
    DbSet<Role> Roles { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<OutboxMessage> OutboxMessages { get; set; }
    DbSet<ApiKey> ApiKeys { get; set; }
    DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    DbSet<EmailTemplate> EmailTemplates { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
    EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
}
