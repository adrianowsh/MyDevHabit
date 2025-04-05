using MyDevHabit.Api.Enums;

namespace MyDevHabit.Api.DTOs.Habits;

public sealed record FrequencyDto
{
    public FrequencyType Type { get; set; }

    public int TimesPerPeriod { get; set; }
}
