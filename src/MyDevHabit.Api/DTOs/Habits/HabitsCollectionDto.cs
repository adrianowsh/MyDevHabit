using MyDevHabit.Api.DTOs.Common;

namespace MyDevHabit.Api.DTOs.Habits;

public sealed record HabitsCollectionDto : ICollectionResponse<HabitDto>
{
    public required IReadOnlyCollection<HabitDto> Data { get; init; }
    public IList<HabitDto> Items { get; init; } = [];
}
