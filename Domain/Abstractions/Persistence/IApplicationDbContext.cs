using Domain.ApiKeys;
using Domain.EmailTemplates;
using Domain.Infrastructure.Outbox;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Domain.Abstractions.Persistence;

public interface IApplicationDbContext : IUnitOfWork
{
    DbSet<Role> Roles { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<OutboxMessage> OutboxMessages { get; set; }
    DbSet<ApiKey> ApiKeys { get; set; }
    DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    DbSet<EmailTemplate> EmailTemplates { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
}
