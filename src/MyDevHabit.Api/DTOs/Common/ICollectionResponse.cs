namespace MyDevHabit.Api.DTOs.Common;

public interface ICollectionResponse<T>
{
    IList<T> Items { get; init; }
}
