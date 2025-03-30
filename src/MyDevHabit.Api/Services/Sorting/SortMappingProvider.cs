namespace MyDevHabit.Api.Services.Sorting;

public sealed class SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinition)
{
    public SortMapping[] GetMappings<TSource, TDestination>()
    {
        SortMappingDefinition<TSource, TDestination>? sortmappingDefinition = sortMappingDefinition
            .OfType<SortMappingDefinition<TSource, TDestination>>()
            .FirstOrDefault();

        if (sortmappingDefinition == null)
        {
            throw new InvalidOperationException(
                $"The mapping from '{typeof(TSource).Name}' into {typeof(TDestination).Name} isn't defined");
        }

        return sortmappingDefinition.Mappings.ToArray();
    }

    public bool Validatemappings<TSource, TDestination>(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return true;
        }

        var sortFields = sort
            .Split(',')
            .Select(s => s.Trim().Split(" ")[0])
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        SortMapping[] mapping = GetMappings<TSource, TDestination>();

        return sortFields.All(field => mapping.Any(m => m.SortField.Equals(field, StringComparison.OrdinalIgnoreCase)));
    }
}

