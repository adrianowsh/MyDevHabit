namespace MyDevHabit.Api.Services.Sorting;

public sealed class SortMappingDefinition<TSource, TDestination> : ISortMappingDefinition
{
    public required IList<SortMapping> Mappings { get; init; }
}
