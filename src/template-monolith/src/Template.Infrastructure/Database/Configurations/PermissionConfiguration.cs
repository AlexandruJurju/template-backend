using Template.Domain.Entities.Users;

namespace Template.Infrastructure.Database.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(permission => permission.Id);

        builder.HasData(Permission.UsersRead);
        builder.HasData(Permission.UsersEdit);
    }
}
