using System.Linq.Expressions;
using MyDevHabit.Api.Entities;
using MyDevHabit.Api.Services.Sorting;
using MyDevHabit.Api.ValueObject;

namespace MyDevHabit.Api.DTOs.Habits;

internal static class HabitQueries
{
    public static Expression<Func<Habit, HabitDto>> ProjectToDto()
    {
        return h => new HabitDto
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            Type = h.Type,
            Frequency = new FrequencyDto
            {
                Type = h.Frequency.Type,
                TimesPerPeriod = h.Frequency.TimesPerPeriod
            },
            Target = new TargetDto
            {
                Value = h.Target.Value,
                Unit = h.Target.Unit
            },
            Status = h.Status,
            IsArchived = h.IsArchived,
            EndDate = h.EndDate,
            Milestone = h.Milestone != null
                ? new MilestoneDto
                {
                    Target = h.Milestone.Target,
                    Current = h.Milestone.Current
                }
                : null,
            CreatedAtUtc = h.CreatedAtUtc,
            UpdatedAtUtc = h.UpdatedAtUtc,
            LastCompletedAtUtc = h.LastCompletedAtUtc
        };
    }

    public static Expression<Func<Habit, HabitWithTagsDto>> ProjectToHabitWithTagsDto()
    {
        return h => new HabitWithTagsDto
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            Type = h.Type,
            Frequency = new FrequencyDto
            {
                Type = h.Frequency.Type,
                TimesPerPeriod = h.Frequency.TimesPerPeriod
            },
            Target = new TargetDto
            {
                Value = h.Target.Value,
                Unit = h.Target.Unit
            },
            Status = h.Status,
            IsArchived = h.IsArchived,
            EndDate = h.EndDate,
            Milestone = h.Milestone != null
                ? new MilestoneDto
                {
                    Target = h.Milestone.Target,
                    Current = h.Milestone.Current
                }
                : null,
            CreatedAtUtc = h.CreatedAtUtc,
            UpdatedAtUtc = h.UpdatedAtUtc,
            LastCompletedAtUtc = h.LastCompletedAtUtc,
            Tags = h.Tags.Select(t => t.Name).ToList()
        };
    }
}

internal static class HabitMappings
{
    public static readonly SortMappingDefinition<HabitDto, Habit> SortMappingDefinition = new()
    {
        Mappings =
            [
                new SortMapping(nameof(HabitDto.Name), nameof(Habit.Name)),
                new SortMapping(nameof(HabitDto.Description), nameof(Habit.Description)),
                new SortMapping(nameof(HabitDto.Type), nameof(Habit.Type)),
                new SortMapping(
                    $"{nameof(HabitDto.Frequency)}.{nameof(FrequencyDto.Type)}",
                    $"{nameof(Habit.Frequency)}.{nameof(Frequency.Type)}"),
                new SortMapping(
                    $"{nameof(HabitDto.Frequency)}.{nameof(FrequencyDto.TimesPerPeriod)}",
                    $"{nameof(Habit.Frequency)}.{nameof(Frequency.TimesPerPeriod)}"),
                 new SortMapping(
                    $"{nameof(HabitDto.Target)}.{nameof(TargetDto.Value)}",
                    $"{nameof(Habit.Target)}.{nameof(Target.Value)}"),
                new SortMapping(
                    $"{nameof(HabitDto.Target)}.{nameof(TargetDto.Unit)}",
                    $"{nameof(Habit.Target)}.{nameof(Target.Unit)}"),
                new SortMapping(nameof(HabitDto.Status), nameof(Habit.Status)),
                new SortMapping(nameof(HabitDto.EndDate), nameof(Habit.EndDate)),
                new SortMapping(nameof(HabitDto.CreatedAtUtc), nameof(Habit.CreatedAtUtc)),
                new SortMapping(nameof(HabitDto.UpdatedAtUtc), nameof(Habit.UpdatedAtUtc)),
                new SortMapping(nameof(HabitDto.LastCompletedAtUtc), nameof(Habit.LastCompletedAtUtc)),
            ]
    };
    public static Habit ToEntity(this CreateHabitDto dto)
    {
        Habit habit = new()
        {
            Id = $"h_{Guid.CreateVersion7()}",
            Name = dto.Name,
            Description = dto.Description ?? string.Empty,
            Type = dto.Type,
            Frequency = new Frequency
            {
                Type = dto.Frequency.Type,
                TimesPerPeriod = dto.Frequency.TimesPerPeriod
            },
            Target = new Target
            {
                Value = dto.Target.Value,
                Unit = dto.Target.Unit
            },
            Status = Enums.HabitStatus.Ongoing,
            IsArchived = false,
            EndDate = dto.EndDate,
            Milestone = dto.Milestone is not null
                ? new Milestone
                {
                    Target = dto.Milestone.Target,
                    Current = 0 // Initialize current progress to 0
                }
                : null
        };

        return habit;
    }
    public static HabitDto ToDto(this Habit habit)
    {
        return new HabitDto
        {
            Id = habit.Id,
            Name = habit.Name,
            Description = habit.Description,
            Type = habit.Type,
            Frequency = new FrequencyDto
            {
                Type = habit.Frequency.Type,
                TimesPerPeriod = habit.Frequency.TimesPerPeriod
            },
            Target = new TargetDto
            {
                Value = habit.Target.Value,
                Unit = habit.Target.Unit
            },
            Status = habit.Status,
            IsArchived = habit.IsArchived,
            EndDate = habit.EndDate,
            Milestone = habit.Milestone != null
                ? new MilestoneDto
                {
                    Target = habit.Milestone.Target,
                    Current = habit.Milestone.Current
                }
                : null,
            CreatedAtUtc = habit.CreatedAtUtc,
            UpdatedAtUtc = habit.UpdatedAtUtc,
            LastCompletedAtUtc = habit.LastCompletedAtUtc
        };
    }
    public static void UpdateFromDto(this Habit habit, UpdateHabitDto dto)
    {
        habit.Name = dto.Name;
        habit.Description = dto.Description ?? string.Empty;
        habit.Type = dto.Type;
        habit.Frequency = new Frequency
        {
            Type = dto.Frequency.Type,
            TimesPerPeriod = dto.Frequency.TimesPerPeriod
        };
        habit.Target = new Target
        {
            Value = dto.Target.Value,
            Unit = dto.Target.Unit
        };
        habit.EndDate = dto.EndDate;
        habit.Milestone = dto.Milestone is not null
            ? new Milestone
            {
                Target = dto.Milestone.Target,
            }
            : null;

        habit.UpdatedAtUtc = DateTime.UtcNow;
    }
}


