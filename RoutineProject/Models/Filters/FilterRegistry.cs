using System.Linq.Expressions;

namespace RoutineProject.Models.Filters;

/// <summary>
/// Represents a filter definition for a specific filterable property on type T.
/// Contains all necessary components to apply filtering logic to queries.
/// </summary>
/// <typeparam name="T">The entity type this filter definition applies to</typeparam>
public class FilterDefinition<T>
{
    public required string Key { get; set; }
    public required Func<T, IEnumerable<string>> ValueExtractor { get; set; }
    public required Expression<Func<T, IEnumerable<string>, bool>> PredicateBuilder { get; set; }
}

/// <summary>
/// Registry for managing filter definitions for type T.
/// Provides methods to register filters, retrieve definitions, and build filter groups from data.
/// </summary>
/// <typeparam name="T">The entity type this registry manages filters for</typeparam>
public class FilterRegistry<T>
{
    private readonly List<FilterDefinition<T>> _definitions = [];

    /// <summary>
    /// Registers a new filter definition in the registry.
    /// </summary>
    /// <param name="key">Unique key identifying the filter</param>
    /// <param name="valueExtractor">Function to extract filterable values from entities</param>
    /// <param name="predicateBuilder">Expression defining how to apply the filter</param>
    /// <exception cref="ArgumentException">Thrown when a filter with the same key already exists</exception>
    public void RegisterFilter(string key,
      Func<T, IEnumerable<string>> valueExtractor,
      Expression<Func<T, IEnumerable<string>, bool>> predicateBuilder)
    {
        _definitions.Add(new FilterDefinition<T>
        {
            Key = key,
            ValueExtractor = valueExtractor,
            PredicateBuilder = predicateBuilder
        });
    }

    public IReadOnlyList<FilterDefinition<T>> Definitions => _definitions.AsReadOnly();

    public FilterDefinition<T>? GetDefinition(string key) =>
        _definitions.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Builds a dictionary of available filter options from a collection of entities.
    /// This is typically used to populate filter dropdowns in UI components.
    /// </summary>
    /// <param name="entities">The collection of entities to extract filter values from</param>
    /// <returns>
    /// Sorted dictionary where keys are filter names and values are distinct, sorted available options
    /// </returns>
    /// <example>
    /// Output:
    /// {
    ///   "Category": ["Electronics", "Clothing", "Books"],
    ///   "Status": ["Active", "Inactive", "Pending"]
    /// }
    /// </example>
    public SortedDictionary<string, IEnumerable<string>> BuildFilterGroups(
         IEnumerable<T> entities)
    {
        var filterGroups = new SortedDictionary<string, IEnumerable<string>>();

        foreach (var filterDef in _definitions)
        {
            var values = entities
                .SelectMany(filterDef.ValueExtractor)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            filterGroups[filterDef.Key] = values;
        }

        return filterGroups;
    }
}