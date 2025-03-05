using MyDevHabit.Api.Enums;

namespace MyDevHabit.Api.Entities;

public sealed class Frequency
{
    public FrequencyType Type { get; set; }

    public int TimesPerPeriod { get; set; }
}



