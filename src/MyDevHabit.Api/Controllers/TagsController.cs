using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MyDevHabit.Api.Database;
using MyDevHabit.Api.DTOs.Habits;
using MyDevHabit.Api.DTOs.Tags;
using MyDevHabit.Api.Entities;

namespace MyDevHabit.Api.Controllers;

[ApiController]
[Route("tags")]
public sealed class TagsController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;

    public TagsController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<TagsCollectionDto>> GetTags()
    {
        List<TagDto> tags = await dbContext
            .Tags
            .Select(TagQueries.ProjectToDto())
            .ToListAsync();

        var tagsCollectionDto = new TagsCollectionDto
        {
            Data = tags
        };

        return Ok(tagsCollectionDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(string id)
    {
        TagDto tag = await dbContext
            .Tags
            .Where(tag => tag.Id == id)
            .Select(TagQueries.ProjectToDto())
            .SingleOrDefaultAsync();

        if (tag is null)
        {
            return NotFound();
        }

        return Ok(tag);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(
        CreateTagDto createtagDto,
        IValidator<CreateTagDto> validator,
        ProblemDetailsFactory problemDetailsFactory)
    {
        FluentValidation.Results.ValidationResult validationResul = await validator.ValidateAsync(createtagDto);

        if (!validationResul.IsValid)
        {
            ProblemDetails problem = problemDetailsFactory.CreateProblemDetails(
               HttpContext,
               StatusCodes.Status400BadRequest);
            problem.Extensions.Add("errors", validationResul.ToDictionary());

            return BadRequest(problem);
        }

        Tag tag = createtagDto.ToEntity();

        bool any = await dbContext.Tags.AnyAsync(tag => tag.Name == createtagDto.Name);

        if (any)
        {
            return Problem(
                detail: $"The tag {tag.Name} already exists",
                 statusCode: StatusCodes.Status409Conflict
            );
        }

        dbContext.Tags.Add(tag);

        await dbContext.SaveChangesAsync();

        TagDto tagDto = tag.ToDto();

        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tagDto);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult<TagDto>> UpdateTag(string id, UpdateTagDto updateTagDto)
    {
        Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(tag => tag.Id == id);

        if (tag is null)
        {
            return NotFound();
        }

        tag.UpdateFromDto(updateTagDto);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTag(string id)
    {
        Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(tag => tag.Id == id);

        if (tag is null)
        {
            return NotFound();
        }

        dbContext.Tags.Remove(tag);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
