using RoutineProject.Models.Interfaces;

namespace RoutineProject.Models.Base;

public class BaseTableDefinitionMeta : ITableDefinition
{
  public IEnumerable<ColumnDefinition> Columns { get; set; } = [];
  public IDictionary<string, IEnumerable<string>> FilterGroups { get; set; } = new Dictionary<string, IEnumerable<string>>();
}
