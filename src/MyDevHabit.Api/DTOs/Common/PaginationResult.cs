using Microsoft.EntityFrameworkCore;
using MyDevHabit.Api.DTOs.Habits;

namespace MyDevHabit.Api.DTOs.Common;

public sealed record PaginationResult<T> : ICollectioResponse<HabitDto>
{
    public IList<HabitDto> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static async Task<PaginationResult<T>> CreateAsync(
      IQueryable<T> query,
        int page,
        int pageSize)
    {
        int totalCount = await query.CountAsync();
        IList<T> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PaginationResult<T>
        {
            Items = (IList<HabitDto>)items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
