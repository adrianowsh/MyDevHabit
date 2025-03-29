using MyDevHabit.Api.Entities;

namespace MyDevHabit.Api.DTOs.Tags;

internal static class TagMappings
{
    public static TagDto ToDto(this Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
            CreatedAtUtc = tag.CreatedAtUtc,
            UpdatedAtUtc = tag.UpdatedAtUtc
        };
    }
    public static Tag ToEntity(this TagDto tagDto)
    {
        return new Tag
        {
            Id = tagDto.Id,
            Name = tagDto.Name,
            Description = tagDto.Description,
            CreatedAtUtc = tagDto.CreatedAtUtc,
            UpdatedAtUtc = tagDto.UpdatedAtUtc
        };
    }

    public static Tag ToEntity(this CreateTagDto createTagDto)
    {
        return new Tag
        {
            Id = $"h_{Guid.CreateVersion7()}",
            Name = createTagDto.Name,
            Description = createTagDto.Description,
            CreatedAtUtc = DateTime.UtcNow
        };
    }


    public static void UpdateFromDto(this Tag tag, UpdateTagDto updateTagDto)
    {
        tag.Name = updateTagDto.Name;
        tag.Description = updateTagDto.Description;
        tag.UpdatedAtUtc = DateTime.UtcNow;
    }
}
