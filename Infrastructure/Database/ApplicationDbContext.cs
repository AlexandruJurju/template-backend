using Domain.Abstractions;
using Domain.Abstractions.Persistence;
using Domain.ApiKeys;
using Domain.EmailTemplates;
using Domain.Infrastructure.Outbox;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options
) : DbContext(options), IApplicationDbContext
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddDomainEventsAsOutboxMessages();

        int result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        var outboxMessages = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                TimeProvider.System.GetUtcNow().UtcDateTime,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)
            ))
            .ToList();

        OutboxMessages.AddRange(outboxMessages);
    }
}