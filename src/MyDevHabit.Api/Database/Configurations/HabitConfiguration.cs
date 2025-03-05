using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyDevHabit.Api.Entities;

namespace MyDevHabit.Api.Database.Configurations;

internal sealed class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    void IEntityTypeConfiguration<Habit>.Configure(EntityTypeBuilder<Habit> builder)
    {

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasMaxLength(500);

        builder.Property(x => x.Name).HasMaxLength(100);

        builder.Property(x => x.Description).HasMaxLength(500);

        builder.OwnsOne(x => x.Frequency);

        builder.OwnsOne(x => x.Target, targetBuilder =>
        {
            targetBuilder.Property(t => t.Unit).HasMaxLength(100);
        });

        builder.OwnsOne(x => x.Milestone);
    }
}
