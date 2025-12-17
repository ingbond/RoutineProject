using Microsoft.EntityFrameworkCore;
using RoutineProject.Context;

namespace RoutineProject.Services;

public class MachinesJobService(MainDbContext mainDbContext)
{
    public async Task UpdateMachinesAsync()
    {
        var machines = await mainDbContext.Machines.Where(x => x.Issues.Any()).ToListAsync();

        foreach (var machine in machines)
        {
            // do something important
        }
    }
}
