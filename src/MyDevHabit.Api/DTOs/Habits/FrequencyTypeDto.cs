using MyDevHabit.Api.Enums;

namespace MyDevHabit.Api.DTOs.Habits;

public sealed record FrequencyTypeDto
{
    public required FrequencyType Type { get; init; }
    public required int TimesPerPeriod { get; init; }
}
