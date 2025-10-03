namespace RoutineProject.Models.Interfaces;

public interface ISelectedFilterValues
{
  public Dictionary<string, IEnumerable<string>>? SelectedFilterValues { get; set; }
}
