using MyDevHabit.Api.DTOs.Common;

namespace MyDevHabit.Api.DTOs.Users;

public sealed record UserDto : ILinksResponse
{
    public required string Id { get; init; }

    public required string Email { get; init; }

    public required string Name { get; init; }

    public required DateTime CreatedAtUtc { get; init; }

    public required DateTime? UpdatedAtUtc { get; init; }

    public List<LinkDto> Links { get; set; } = [];
}
