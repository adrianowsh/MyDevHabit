namespace MyDevHabit.Api.DTOs.Common;

public sealed class CollectionResponse<T> : ICollectionResponse<T>, ILinksResponse
{
    public IList<T> Items { get; init; } = [];
    public List<LinkDto> Links { get; set; } = [];
}

