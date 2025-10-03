using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using RoutineProject.Context;
using RoutineProject.Services;
using RoutineProject.Settings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Services
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<SqlSettings>(builder.Configuration.GetSection("SqlSettings"));
builder.Services.AddScoped<MachinesJobService>();
builder.Services.AddScoped<MachinesService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();

using (var scope = app.Services.CreateScope())
{
    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    #region AutoMapper
    var config = new MapperConfiguration(cfg =>
    {
        cfg.AddMaps(Assembly.GetExecutingAssembly());
        // https://github.com/AutoMapper/AutoMapper.Collection
        cfg.AddCollectionMappers();
    }, loggerFactory);
    var mapper = config.CreateMapper();
    builder.Services.AddSingleton(mapper);
    #endregion

    await using var dbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
    dbContext.Database.SetCommandTimeout(3600);
    dbContext.Database.Migrate();
}

app.MapControllers();

app.Run();
