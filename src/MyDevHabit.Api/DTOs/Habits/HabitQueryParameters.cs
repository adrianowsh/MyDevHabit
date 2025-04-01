using Microsoft.AspNetCore.Mvc;
using MyDevHabit.Api.Enums;

namespace MyDevHabit.Api.DTOs.Habits;

public sealed record HabitQueryParameters
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
