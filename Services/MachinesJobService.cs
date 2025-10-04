using Microsoft.EntityFrameworkCore;
using RoutineProject.Context;

namespace RoutineProject.Services;

public class MachinesJobService
{
    private MainDbContext _mainDbContext;

    public MachinesJobService(MainDbContext mainDbContext)
    {
        _mainDbContext = mainDbContext;
    }

    public async Task UpdateMachinesAsync()
    {
        var machines = await _mainDbContext.Machines.Where(x => x.Issues.Any()).ToListAsync();

        foreach (var machine in machines)
        {
            // do something important
        }
    }
}
