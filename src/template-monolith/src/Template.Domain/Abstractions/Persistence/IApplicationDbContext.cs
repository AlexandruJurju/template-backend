using Template.Common.SharedKernel.Infrastructure.Outbox;
using Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;
using Template.Domain.Entities.ApiKeys;
using Template.Domain.Entities.Users;

namespace Template.Domain.Abstractions.Persistence;

public interface IApplicationDbContext : IUnitOfWork
{
    DbSet<Role> Roles { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<OutboxMessage> OutboxMessages { get; set; }
    DbSet<ApiKey> ApiKeys { get; set; }
    DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
    EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
}
