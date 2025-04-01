using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Dynamic.Core;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDevHabit.Api.Database;
using MyDevHabit.Api.DTOs.Common;
using MyDevHabit.Api.DTOs.Habits;
using MyDevHabit.Api.Entities;
using MyDevHabit.Api.Services;
using MyDevHabit.Api.Services.Sorting;

namespace MyDevHabit.Api.Controllers;

[Route("habits")]
[ApiController]
public sealed class HabitsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHabits(
        [FromQuery] HabitQueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapeService dataShapeService)
    {
        query.Search ??= query.Search?.Trim().ToLowerInvariant();

        if (!sortMappingProvider.Validatemappings<HabitDto, Habit>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided sort parameter ins't valid: '{query.Sort}'");
        }

        if (!dataShapeService.Validate<HabitDto>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shapping fields parameter ins't valid: '{query.Fields}'");
        }

        //Expression<Func<Habit, object>> orderBy = query.Sort switch
        //{
        //    "name" => p => p.Name,
        //    "description" => p => p.Description ?? string.Empty,
        //    "type" => p => p.Type,
        //    "status" => p => p.Status,
        //    _ => p => p.Name
        //};

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<HabitDto, Habit>();

        IQueryable<HabitDto> habitQuery = dbContext.Habits.AsNoTracking()
            .Where(p => query.Search == null ||
                        p.Name.Contains(query.Search) ||
                        p.Description != null && p.Description.Contains(query.Search))
            .Where(p => query.Type == null || p.Type == query.Type)
            .Where(p => query.Status == null || p.Status == query.Status)
            .ApplySort(query.Sort, sortMappings)
            .Select(HabitQueries.ProjectToDto());

        int totalCount = await habitQuery.CountAsync();

        List<HabitDto> habits = await habitQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        PaginationResult<ExpandoObject> paginationResult = new()
        {
            Items = dataShapeService.ShapeCollectionData(habits, query.Fields),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return Ok(paginationResult);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHabit(
        string id,
        string? fields,
        DataShapeService dataShapeService)
    {
        HabitWithTagsDto? habit =
            await dbContext.Habits
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(HabitQueries.ProjectToHabitWithTagsDto())
                .FirstOrDefaultAsync();

        if (!dataShapeService.Validate<HabitWithTagsDto>(fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shapping fields parameter ins't valid: '{fields}'");
        }

        if (habit == null)
        {
            return NotFound();
        }

        ExpandoObject shapedHabitDto = dataShapeService.ShapeData(habit, fields);

        return Ok(shapedHabitDto);
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto>> CreateHabit(
        CreateHabitDto createHabitDto,
        IValidator<CreateHabitDto> validator)

    {
        await validator.ValidateAndThrowAsync(createHabitDto);

        Habit habit = createHabitDto.ToEntity();

        await dbContext.Habits.AddAsync(habit);

        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHabit), new { id = habit.Id }, habit.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<HabitDto>> UpdateHabit(string id, UpdateHabitDto updateHabitDto)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(p => p.Id == id);

        if (habit == null)
        {
            return NotFound();
        }

        habit.UpdateFromDto(updateHabitDto);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<HabitDto>> PatchHabit(string id, JsonPatchDocument<HabitDto> patchDocument)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(p => p.Id == id);

        if (habit == null)
        {
            return NotFound();
        }

        HabitDto habitDto = habit.ToDto();

        patchDocument.ApplyTo(habitDto, ModelState);

        if (!TryValidateModel(habitDto))
        {
            return ValidationProblem(ModelState);
        }

        habit.Name = habitDto.Name;
        habit.Description = habitDto.Description ?? string.Empty;
        habit.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteHabit(string id)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(p => p.Id == id);

        if (habit == null)
        {
            return NotFound();
        }

        dbContext.Habits.Remove(habit);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

}
