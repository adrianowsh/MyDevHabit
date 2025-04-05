using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyDevHabit.Api.Entities;

namespace MyDevHabit.Api.Database.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasMaxLength(500);

        builder.Property(x => x.Email).HasMaxLength(300);

        builder.Property(x => x.Name).HasMaxLength(300);

        builder.Property(x => x.CreatedAtUtc);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);

        builder.Property(x => x.IdentityId).HasMaxLength(500);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasIndex(x => x.IdentityId).IsUnique();
    }
}

