﻿using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Token).HasMaxLength(256);

        builder.HasIndex(e => e.Token).IsUnique();

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);
    }
}
