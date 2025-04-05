using System.Net.Mime;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MyDevHabit.Api.Database;
using MyDevHabit.Api.DTOs.Common;
using MyDevHabit.Api.DTOs.Habits;
using MyDevHabit.Api.DTOs.Tags;
using MyDevHabit.Api.Entities;
using MyDevHabit.Api.Services;

namespace MyDevHabit.Api.Controllers;

[ApiController]
[Route("tags")]
//[Produces(
// MediaTypeNames.Application.Json,
// CustomMediaTypeNames.Application.JsonV1,
// CustomMediaTypeNames.Application.HateoasJson,
// CustomMediaTypeNames.Application.HateoasJsonV1)]

public sealed class TagsController(
    ApplicationDbContext dbContext,
    LinkService linkService) : ControllerBase
{
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
    public async Task<ActionResult<TagDto>> GetTag(string id, [FromHeader] AcceptHeaderDto acceptHeader)
    {
        TagDto? tag = await dbContext
            .Tags
            .Where(tag => tag.Id == id)
            .Select(TagQueries.ProjectToDto())
            .SingleOrDefaultAsync();

        if (tag is null)
        {
            return NotFound();
        }

        if (acceptHeader.IncludeLinks)
        {
            tag.Links = CreateLinksForTag(id);
        }
        return Ok(tag);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(
        CreateTagDto createtagDto,
        IValidator<CreateTagDto> validator,
        ProblemDetailsFactory problemDetailsFactory)
    {
        ValidationResult validationResul = await validator.ValidateAsync(createtagDto);

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


    private List<LinkDto> CreateLinksForTag(string id)
    {
        List<LinkDto> links =
        [
            linkService.Create(nameof(GetTag), "self", HttpMethods.Get, new { id }),
            linkService.Create(nameof(UpdateTag), "update", HttpMethods.Put, new { id }),
            linkService.Create(nameof(DeleteTag), "delete", HttpMethods.Delete, new { id }),

        ];

        return links;
    }
}
