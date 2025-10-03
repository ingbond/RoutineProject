using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoutineProject.Entities;
using RoutineProject.Entities.Base;
using RoutineProject.Settings;

namespace RoutineProject.Context;

public class MainDbContext : DbContext
{
    public readonly IWebHostEnvironment env;
    public readonly IOptions<SqlSettings> sqlSettings;

    public MainDbContext(IOptions<SqlSettings> sqlSettings, IWebHostEnvironment env)
    {
        this.sqlSettings = sqlSettings;
        this.env = env;
    }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var settingsValue = sqlSettings.Value;
        var connectionStringBuilder = new SqlConnectionStringBuilder
        {
            DataSource = settingsValue.DataSource,
            UserID = settingsValue.Username,
            Password = settingsValue.Password,
            MultipleActiveResultSets = true,
            Encrypt = false
        };
        connectionStringBuilder["Database"] = settingsValue.Database;
        var connectionString = connectionStringBuilder.ToString();

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(connectionString, options => options.EnableRetryOnFailure())
              .EnableSensitiveDataLogging()
              .EnableDetailedErrors();
            if (settingsValue.LogToConsole)
            {
                optionsBuilder.LogTo(Console.WriteLine);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.Id))
                    .HasDefaultValueSql("(newid())");
            }

        modelBuilder.Entity<Issue>(entity =>
        {
            entity.HasQueryFilter(p => !p.IsDeleted);
            entity.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<Machine>(entity =>
        {
            entity.ToTable("Machine");
        });
    }
}
