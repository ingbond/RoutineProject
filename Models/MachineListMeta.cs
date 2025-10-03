using RoutineProject.Models.Base;

namespace RoutineProject.Models;

public class MachineListMeta : BaseTableDefinitionMeta
{
  public IEnumerable<Guid> FavoriteMachineIds { get; set; } = [];
}
