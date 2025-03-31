using MyDevHabit.Api.DTOs.Common;

namespace MyDevHabit.Api.DTOs.Tags;


public sealed record TagsCollectionDto : ICollectioResponse<TagDto>
{
    public required IReadOnlyCollection<TagDto> Data { get; init; }
    public IList<TagDto> Items { get; init; } = [];
}

public sealed record TagDto
{
    public required string Id { get; init; } = string.Empty;
    public required string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
