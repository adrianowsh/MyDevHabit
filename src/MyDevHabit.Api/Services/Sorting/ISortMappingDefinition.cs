namespace MyDevHabit.Api.Services.Sorting;

public interface ISortMappingDefinition
{
    IList<SortMapping> Mappings { get; init; }
}
