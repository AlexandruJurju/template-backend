using Template.Common.SharedKernel.Infrastructure.Outbox;

namespace Template.Infrastructure.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            // Just for pgsql
            .HasColumnType("jsonb");
    }
}
