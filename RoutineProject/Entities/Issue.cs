using RoutineProject.Entities.Base;

namespace RoutineProject.Entities;

public class Issue : BaseEntity
{
    public required string Title { get; set; }
    public IssueStatus Status { get; set; } = IssueStatus.Open;
    public bool IsImportant { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public Guid MachineId { get; set; }
    public virtual Machine Machine { get; set; } = null!;
}

public enum IssueStatus
{
    Open, InProgress, Resolved
}
