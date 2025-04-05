using MyDevHabit.Api.DTOs.Common;

namespace MyDevHabit.Api.DTOs.Tags;

public sealed record TagDto : ILinksResponse
{
    public required string Id { get; init; } = string.Empty;
    public required string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public List<LinkDto> Links { get; set; } = [];
}
