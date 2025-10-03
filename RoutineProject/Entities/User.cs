using RoutineProject.Entities.Base;

namespace RoutineProject.Entities;

public class User : BaseEntity
{
  public required string Email { get; set; }
  public required string Username { get; set; }
}
