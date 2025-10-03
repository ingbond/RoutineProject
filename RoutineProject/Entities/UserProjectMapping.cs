namespace RoutineProject.Entities;

public class UserProjectMapping
{
  public Guid UserId { get; set; }
  public virtual User User { get; set; } = null!;
  public Guid ProjectId { get; set; }
  public virtual Project Project { get; set; } = null!;
  public bool IsManuallyAdded { get; set; } = false;
  public UserRole? Role { get; set; }
}

public enum UserRole
{
  User, Technician, Supervisor, Manager
}