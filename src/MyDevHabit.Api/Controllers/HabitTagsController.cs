using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDevHabit.Api.Database;
using MyDevHabit.Api.DTOs.HabitTags;
using MyDevHabit.Api.Entities;

namespace MyDevHabit.Api.Controllers;

[ApiController]
[Route("habits/{habitId}/tags")]
public sealed class HabitTagsController(ApplicationDbContext dbContext) : ControllerBase
{
    public static readonly string Name = nameof(HabitTagsController).Replace("Controller", string.Empty, StringComparison.InvariantCultureIgnoreCase);

    [HttpPut]
    public async Task<ActionResult> UpsertHabitTags(string habitId, UpsertHabitTagsDto upsertHabitTagsDto)
    {

        Habit? habit = await dbContext.Habits
            .Include(t => t.HabitTags)
            .FirstOrDefaultAsync(h => h.Id == habitId);

        if (habit is null)
        {
            return NotFound();
        }

        var currentTagIds = habit.HabitTags.Select(t => t.TagId).ToHashSet();

        if (currentTagIds.SetEquals(upsertHabitTagsDto.TagIds))
        {
            return NoContent();
        }

        List<string> existingTags = await dbContext.Tags
            .Where(t => upsertHabitTagsDto.TagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync();

        if (existingTags.Count != upsertHabitTagsDto.TagIds.Count)
        {
            return BadRequest("One or more tags is invalid");
        }

        habit.HabitTags.RemoveAll(ht => !upsertHabitTagsDto.TagIds.Contains(ht.TagId));

        string[] tagIdsToAdd = upsertHabitTagsDto.TagIds.Except(currentTagIds).ToArray();

        habit.HabitTags.AddRange(tagIdsToAdd.Select(tagId => new HabitTag
        {
            HabitId = habitId,
            TagId = tagId,
            CreatedAtUtc = DateTime.UtcNow
        }));

        await dbContext.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{tagId}")]
    public async Task<ActionResult> DeleteHabitTags(string habitId, string tagId)
    {
        HabitTag? habitsTags = await dbContext.HabitsTags
            .FirstOrDefaultAsync(ht => ht.HabitId == habitId && ht.TagId == tagId);

        if (habitsTags == null)
        {
            return NotFound();
        }

        dbContext.HabitsTags.Remove(habitsTags);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
