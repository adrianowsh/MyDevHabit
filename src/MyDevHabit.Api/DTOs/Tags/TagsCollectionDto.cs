using MyDevHabit.Api.DTOs.Common;

namespace MyDevHabit.Api.DTOs.Tags;

public sealed record TagsCollectionDto : ICollectionResponse<TagDto>
{
    public required IReadOnlyCollection<TagDto> Data { get; init; }
    public IList<TagDto> Items { get; init; } = [];
}
