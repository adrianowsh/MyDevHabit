using MyDevHabit.Api.Enums;
using MyDevHabit.Api.ValueObject;

namespace MyDevHabit.Api.Entities;

public sealed class Habit
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public HabitType Type { get; set; }
    public required Frequency Frequency { get; set; }
    public required Target Target { get; set; }
    public bool IsArchived { get; set; }
    public DateOnly? EndDate { get; set; }
    public Milestone? Milestone { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? LastCompletedAtUtc { get; set; }
    public HabitStatus Status { get; set; }
}




