using RoutineProject.Models.Base;

namespace RoutineProject.Models.Interfaces
{
    public interface ITableDefinition
    {
        public IEnumerable<ColumnDefinition> Columns { get; set; }
    }
}
