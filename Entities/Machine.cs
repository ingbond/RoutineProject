using RoutineProject.Entities.Base;

namespace RoutineProject.Entities;

public class Machine : BaseEntity
{
    public required string SerialNumber { get; set; }
    public string? Location { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<Issue> Issues { get; set; } = [];
}