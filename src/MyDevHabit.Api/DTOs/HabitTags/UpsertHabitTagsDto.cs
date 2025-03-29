namespace MyDevHabit.Api.DTOs.HabitTags;

public sealed record UpsertHabitTagsDto
{
    public required ICollection<string> TagIds { get; init; }
}
