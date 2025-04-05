using Microsoft.AspNetCore.Mvc;
using MyDevHabit.Api.DTOs.Common;
using MyDevHabit.Api.Enums;

namespace MyDevHabit.Api.DTOs.Habits;

public sealed record HabitsQueryParameters : AcceptHeaderDto
{
    [FromQuery(Name = "q")]
    public string? Search { get; set; }
    public HabitType? Type { get; init; }
    public HabitStatus? Status { get; init; }
    public string? Sort { get; init; }
    public int Page { get; init; } = 1;
    public string? Fields { get; init; }
    public int PageSize { get; init; } = 10;
}


public sealed record HabitQueryParameters : AcceptHeaderDto
{
    public string? Fields { get; set; }
}
