using RoutineProject.Models.Interfaces;

namespace RoutineProject.Models.Pagination;

public class GetMachinesListPaginationMeta : SortingPaginationMeta, ISelectedFilterValues
{
  public Dictionary<string, IEnumerable<string>>? SelectedFilterValues { get; set; }
}