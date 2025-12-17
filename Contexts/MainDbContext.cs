using Microsoft.EntityFrameworkCore;
using RoutineProject.Entities;

namespace RoutineProject.Context;

public class MainDbContext : DbContext
{
    public virtual DbSet<Issue> Issues { get; set; } = null!;
    public virtual DbSet<Machine> Machines { get; set; } = null!;
    public virtual DbSet<Project> Projects { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<UserProjectMapping> UserProjectMappings { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>()
          .HavePrecision(3);
        // store enums as string in the db
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Issue>(entity =>
        {
            entity.HasQueryFilter(p => !p.IsDeleted);
            entity.HasIndex(x => x.Status);
        });
    }
}