using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyDevHabit.Api.Entities;

namespace MyDevHabit.Api.Database.Configurations;

internal sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(builder => builder.Id);

        builder.Property(builder => builder.Id).HasMaxLength(500);

        builder.Property(builder => builder.Name).IsRequired().HasMaxLength(50);

        builder.Property(builder => builder.Description).HasMaxLength(500);

        builder.HasIndex(builder => builder.Name).IsUnique();
    }
}
