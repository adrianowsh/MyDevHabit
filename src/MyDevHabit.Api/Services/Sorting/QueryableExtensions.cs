using System.Linq.Dynamic.Core;

namespace MyDevHabit.Api.Services.Sorting;

internal static class QueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> query,
        string? sort,
        SortMapping[] mappings,
        string defaultOrderBy = "Id")
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderBy(defaultOrderBy);
        }

        string[] sortFields = sort.Split(',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        var orderByparts = new List<string>(0);
        foreach (string field in sortFields)
        {
            (string sortField, bool isDescending) = ParseSortField(field);

            SortMapping mapping = mappings
                .First(m => m.SortField.Equals(sortField, StringComparison.OrdinalIgnoreCase));

            string direction = (isDescending, mapping.Reverse) switch
            {
                (true, true) => "ASC",
                (true, false) => "DESC",
                (false, true) => "DESC",
                (false, false) => "ASC"
            };

            orderByparts.Add($"{mapping.Propertyname} {direction}");
        }

        string orderby = string.Join(",", orderByparts);

        return query.OrderBy(orderby);
    }

    private static (string SortField, bool IsDescending) ParseSortField(string field)
    {
        string[] parts = field.Split(' ');
        string sortfield = parts[0];
        bool isDescending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        return (sortfield, isDescending);
    }
}
