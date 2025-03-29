using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyDevHabit.Api.Entities;

namespace MyDevHabit.Api.Database.Configurations;

internal sealed class HabitTagConfiguration : IEntityTypeConfiguration<HabitTag>
{
    public void Configure(EntityTypeBuilder<HabitTag> builder)
    {
        builder.HasKey(x => new { x.HabitId, x.TagId });

        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(x => x.TagId);

        builder.HasOne<Habit>()
            .WithMany(x => x.HabitTags)
            .HasForeignKey(x => x.HabitId);

    }
}
