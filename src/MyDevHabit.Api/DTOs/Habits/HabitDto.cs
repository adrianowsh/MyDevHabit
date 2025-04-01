using MyDevHabit.Api.DTOs.Common;
using MyDevHabit.Api.Enums;

namespace MyDevHabit.Api.DTOs.Habits;

public sealed record HabitsCollectionDto : ICollectionResponse<HabitDto>
{
    public required IReadOnlyCollection<HabitDto> Data { get; init; }
    public IList<HabitDto> Items { get; init; } = [];
}

public sealed record HabitDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required HabitType Type { get; init; }
    public required HabitStatus Status { get; init; }
    public required FrequencyDto Frequency { get; init; }
    public required TargetDto Target { get; init; }
    public required bool IsArchived { get; init; }
    public DateOnly? EndDate { get; init; }
    public MilestoneDto? Milestone { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public DateTime? LastCompletedAtUtc { get; init; }

}

public sealed record FrequencyDto
{
    public FrequencyType Type { get; set; }

    public int TimesPerPeriod { get; set; }
}

public sealed record FrequencyTypeDto
{
    public required FrequencyType TYpe { get; init; }
    public required int TimesPerPeriod { get; init; }
}

public sealed record TargetDto
{
    public required int Value { get; init; }
    public required string Unit { get; init; }
}

public sealed record MilestoneDto
{
    public required int Target { get; init; }
    public required int Current { get; init; }
}
