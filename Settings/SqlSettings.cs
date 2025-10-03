namespace RoutineProject.Settings;

public class SqlSettings
{
  public string Database { get; set; } = null!;
  public string DataSource { get; set; } = null!;
  public string Password { get; set; } = null!;
  public string Username { get; set; } = null!;
  public bool LogToConsole { get; set; } = false;
}