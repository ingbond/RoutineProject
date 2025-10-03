namespace RoutineProject.Models;

public class PaginationResult<T> : PaginationMeta where T : class
{
    public PaginationResult(IEnumerable<T> items, int totalRecords, PaginationMeta paginationMeta)
    {
        Items = items;
        TotalRecords = totalRecords;
        PageSize = paginationMeta.PageSize;
        PageNumber = paginationMeta.PageNumber;

        TotalPages = Convert.ToInt32(Math.Ceiling(TotalRecords / (double)PageSize));
        HasNextPage = PageNumber < TotalPages;
        HasPreviousPage = PageNumber > 1;
    }

    public IEnumerable<T> Items { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}