using Template.Domain.Entities.Users;

namespace Template.Infrastructure.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(role => role.Id);

        builder.HasMany(role => role.Users)
            .WithOne(user => user.Role);

        builder.HasMany(role => role.Permissions)
            .WithMany();

        builder.HasData(Role.Member);
        builder.HasData(Role.Manager);
        builder.HasData(Role.Admin);
    }
}
