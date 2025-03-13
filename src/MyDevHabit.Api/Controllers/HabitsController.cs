using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDevHabit.Api.Database;
using MyDevHabit.Api.DTOs.Habits;
using MyDevHabit.Api.Entities;

namespace MyDevHabit.Api.Controllers;

[Route("habits")]
[ApiController]
public sealed class HabitsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits()
    {
        List<HabitDto> habits = await dbContext
            .Habits
            .Select(HabitQueries.ProjectToDto())
            .ToListAsync();

        var habitsCollectionDto = new HabitsCollectionDto
        {
            Data = habits
        };

        return Ok(habitsCollectionDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HabitDto>> GetHabit(string id)
    {
        HabitDto? habit =
            await dbContext.Habits
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(HabitQueries.ProjectToDto())
                .FirstOrDefaultAsync();

        if (habit == null)
        {
            return NotFound();
        }

        return Ok(habit);
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto>> CerateHabit(CreateHabitDto createHabitDto)
    {
        Habit habit = createHabitDto.ToEntity();

        await dbContext.Habits.AddAsync(habit);

        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHabit), new { id = habit.Id }, habit.ToDto());
    }
}
