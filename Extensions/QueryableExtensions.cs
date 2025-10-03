using LinqKit;
using RoutineProject.Models.Filters;
using RoutineProject.Models.Interfaces;
using System.Linq.Expressions;

namespace RoutineProject.Extensions;

public static class QueryableExtensions
{

    /// <summary>
    /// Filters an IQueryable using a FilterRegistry for more complex and type-safe filtering scenarios.
    /// This method uses pre-registered filter definitions with custom predicate logic.
    /// </summary>
    /// <typeparam name="T">The entity type being queried</typeparam>
    /// <param name="query">The base query to apply filters to</param>
    /// <param name="filterRegistry">Registry containing predefined filter definitions</param>
    /// <param name="selection">Object containing the selected filter values</param>
    /// <returns>Filtered IQueryable with all applicable filters applied</returns>
    /// <remarks>
    /// This approach is more performant than the dictionary version for complex filters
    /// as it doesn't require runtime compilation of expressions.
    /// </remarks>
    public static IQueryable<T> TryFilterQueryByFilterGroups<T>(
        this IQueryable<T> query,
        FilterRegistry<T> filterRegistry,
        ISelectedFilterValues selection)
    {
        if (selection.SelectedFilterValues == null) return query;

        var predicate = PredicateBuilder.New<T>(true);

        foreach (var filterGroup in selection.SelectedFilterValues)
        {
            var filterDefinition = filterRegistry.GetDefinition(filterGroup.Key);
            if (filterDefinition == null) continue;

            var selectedValuesParam = Expression.Constant(filterGroup.Value, typeof(IEnumerable<string>));
            var predicateBody = Expression.Invoke(
                filterDefinition.PredicateBuilder,
                filterDefinition.PredicateBuilder.Parameters[0],
                selectedValuesParam);

            var lambda = Expression.Lambda<Func<T, bool>>(predicateBody, filterDefinition.PredicateBuilder.Parameters[0]);
            predicate = predicate.And(lambda);
        }

        return query.AsExpandable().Where(predicate);
    }

    /// <summary>
    /// Filters an IQueryable based on selected filter values using a simple property mapping dictionary.
    /// This method performs string-based containment checks on specified properties.
    /// </summary>
    /// <typeparam name="T">The entity type being queried</typeparam>
    /// <param name="query">The base query to apply filters to</param>
    /// <param name="filterGroupMapping">Dictionary mapping filter keys to property selectors</param>
    /// <param name="selection">Object containing the selected filter values</param>
    /// <returns>Filtered IQueryable with all applicable filters applied</returns>
    /// <remarks>
    /// This method compiles expressions dynamically, which may have performance implications
    /// for large datasets. Consider using the FilterRegistry version for complex filtering scenarios.
    /// </remarks>
    public static IQueryable<T> TryFilterQueryByFilterGroups<T>(
        this IQueryable<T> query,
        Dictionary<string, Expression<Func<T, string?>>> filterGroupMapping,
        ISelectedFilterValues selection)
    {
        if (selection.SelectedFilterValues == null) return query;

        var predicate = PredicateBuilder.New<T>(true);

        foreach (var item in selection.SelectedFilterValues)
        {
            filterGroupMapping.TryGetValue(item.Key, out var propertySelector);
            if (propertySelector == null) continue;
            predicate = predicate.And(x => item.Value.Contains(propertySelector.Compile()(x)!));
        }
        return query.AsExpandable().Where(predicate);
    }
}
