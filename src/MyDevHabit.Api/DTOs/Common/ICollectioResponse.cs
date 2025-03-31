namespace MyDevHabit.Api.DTOs.Common;

public interface ICollectioResponse<T>
{
    IList<T> Items { get; init; }
}
