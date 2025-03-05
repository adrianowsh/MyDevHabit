using MyDevHabit.Api.Enums;

namespace MyDevHabit.Api.Entities;

internal sealed class Frequency
{
    public FrequencyType FrequencyType { get; set; }

    public int TimesPerPeriod { get; set; }
}



