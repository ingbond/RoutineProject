using Microsoft.AspNetCore.Mvc;

namespace RoutineProject.Models;

public class PaginationMeta
{
    /// <summary> The page number to retrieve </summary>
    [FromQuery]
    public int PageNumber { get; set; } = 1;

    /// <summary> The number of items per page </summary>
    [FromQuery]
    public int PageSize { get; set; } = 10;
}

/// <summary>
///     This is a convenience class that has additional parameters for time based filtering and textual search.
/// </summary>
public class ExtendedPaginationMeta : PaginationMeta
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? SearchTerm { get; set; }
}

public class SortingPaginationMeta : ExtendedPaginationMeta
{
    public string? SortHeader { get; set; }
    public SortDirectionEnum SortDirection { get; set; } = SortDirectionEnum.Asc;
}

public enum SortDirectionEnum
{
    Asc,
    Desc
}
