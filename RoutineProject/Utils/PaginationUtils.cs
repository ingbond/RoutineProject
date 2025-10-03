using RoutineProject.Models;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace RoutineProject.Utils;

public static class PaginationUtils
{
    /// <summary>
    ///     Paginates the response based on the page size and page number
    /// </summary>
    /// <param name="source"></param>
    /// <param name="paginationMeta"> Paginated page number between 1 .. MaxPage and Paginated page size </param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IQueryable<TSource> Paginate<TSource>([NotNull] this IQueryable<TSource> source,
        PaginationMeta paginationMeta)
    {
        if (paginationMeta.PageNumber < 1) throw new ArgumentOutOfRangeException("PageNumber must be >= 1");

        if (paginationMeta.PageSize < 1) throw new ArgumentOutOfRangeException("PageSize must be >= 1");

        return source.Skip(paginationMeta.PageSize * (paginationMeta.PageNumber - 1))
            .Take(paginationMeta.PageSize);
    }

    /// <summary>
    ///     Sorting the response based on the sorting pagination info
    /// </summary>
    /// <param name="source"></param>
    /// <param name="paginationMeta"> Pagination info </param>
    /// <param name="sortHeaderMapping"> Relation between sort header and class property ex: ["propName"] = x => x.ObjectProp </param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IQueryable<TSource> MakeSorting<TSource>([NotNull] this IQueryable<TSource> source,
        SortingPaginationMeta paginationMeta,
        Dictionary<string, Expression<Func<TSource, dynamic>>> sortHeaderMapping)
    {
        if (!string.IsNullOrEmpty(paginationMeta.SortHeader) &&
            sortHeaderMapping.ContainsKey(paginationMeta.SortHeader))
            source = paginationMeta.SortDirection == SortDirectionEnum.Asc
                ? source.OrderBy(sortHeaderMapping[paginationMeta.SortHeader])
                : source.OrderByDescending(sortHeaderMapping[paginationMeta.SortHeader]);

        return source;
    }
}