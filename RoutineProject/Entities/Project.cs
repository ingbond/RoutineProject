using RoutineProject.Entities.Base;

namespace RoutineProject.Entities;

public class Project : BaseEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<Machine> Machines { get; set; } = [];
}