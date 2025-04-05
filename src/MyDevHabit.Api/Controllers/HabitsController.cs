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
public sealed class HabitsController(ApplicationDbContext dbContext, LinkService linkService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHabits(
        [FromQuery] HabitsQueryParameters query,
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
                detail: $"The provided data shaping fields parameter ins't valid: '{query.Fields}'");
        }

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
            Items = dataShapeService.ShapeCollectionData(
                habits,
                query.Fields,
               query.IncludeLinks ? h => CreateLinksForHabit(h.Id, query.Fields) : null),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
        };

        if (query.IncludeLinks)
        {
            paginationResult.Links = CreateLinksForHabits(
                query,
                paginationResult.HasNextPage,
                paginationResult.HasPreviousPage);
        }

        return Ok(paginationResult);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHabit(
        string id,
        string? fields,
        DataShapeService dataShapeService,
        [FromQuery] HabitQueryParameters query)
    {

        if (!dataShapeService.Validate<HabitWithTagsDto>(fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields parameter ins't valid: '{fields}'");
        }

        HabitWithTagsDto? habit =
            await dbContext.Habits
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(HabitQueries.ProjectToHabitWithTagsDto())
                .FirstOrDefaultAsync();

        if (habit == null)
        {
            return NotFound();
        }

        ExpandoObject shapedHabitDto = dataShapeService.ShapeData(habit, fields);

        if (query.IncludeLinks)
        {
            ((IDictionary<string, object?>)shapedHabitDto)[nameof(ILinksResponse.Links)] =
                CreateLinksForHabit(id, query.Fields);
        }

        return Ok(shapedHabitDto);
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto>> CreateHabit(
        CreateHabitDto createHabitDto,
        [FromHeader] AcceptHeaderDto acceptHeaderDto,
        IValidator<CreateHabitDto> validator)

    {
        await validator.ValidateAndThrowAsync(createHabitDto);

        Habit habit = createHabitDto.ToEntity();

        await dbContext.Habits.AddAsync(habit);

        await dbContext.SaveChangesAsync();

        HabitDto habitDto = habit.ToDto();

        if (acceptHeaderDto.IncludeLinks)
        {
            habitDto.Links = CreateLinksForHabit(habit.Id, null);
        }

        return CreatedAtAction(nameof(GetHabit), new { id = habit.Id }, habitDto);
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

    private List<LinkDto> CreateLinksForHabits(
        HabitsQueryParameters parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            linkService.Create(nameof(GetHabit), "self", HttpMethods.Get, new
            {
                page = parameters.Page,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                type = parameters.Type,
                status = parameters.Status
            }),

            linkService.Create(nameof(CreateHabit), "create", HttpMethods.Post),
        ];


        if (hasNextPage)
        {
            links.Add(linkService.Create(nameof(GetHabit), "next-page", HttpMethods.Get, new
            {
                page = parameters.Page + 1,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                type = parameters.Type,
                status = parameters.Status
            }));

            if (hasPreviousPage)
            {
                links.Add(linkService.Create(nameof(GetHabit), "previous-page", HttpMethods.Get, new
                {
                    page = parameters.Page - 1,
                    pageSize = parameters.PageSize,
                    fields = parameters.Fields,
                    q = parameters.Search,
                    sort = parameters.Sort,
                    type = parameters.Type,
                    status = parameters.Status
                }));
            }
        }

        return links;
    }

    private List<LinkDto> CreateLinksForHabit(string id, string? fields)
    {
        List<LinkDto> links =
        [
            linkService.Create(nameof(GetHabit), "self", HttpMethods.Get, new { id, fields }),
            linkService.Create(nameof(UpdateHabit), "update", HttpMethods.Put, new { id }),
            linkService.Create(nameof(PatchHabit), "partial-update", HttpMethods.Patch, new { id }),
            linkService.Create(nameof(DeleteHabit), "delete", HttpMethods.Delete, new { id }),
            linkService.Create(
                nameof(HabitTagsController.UpsertHabitTags),
                "upsert-tags",
                HttpMethods.Put,
                new { habitId = id },
                HabitTagsController.Name)
        ];

        return links;
    }
}
