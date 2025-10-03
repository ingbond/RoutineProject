using RoutineProject.HostedServices.Base;
using RoutineProject.Services;

namespace RoutineProject.HostedServices;

public class MachinesHostedService : BaseHostedService
{
    private const int _taskDebounceTimeoutMs = 10 * 60000; // every 10 min

    public MachinesHostedService(ILogger<MachinesHostedService> logger, IServiceScopeFactory serviceScopeFactory) : 
        base(logger, serviceScopeFactory, _taskDebounceTimeoutMs)
    {
    }

    protected override async Task DoWorkAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var schedulerService = scope.ServiceProvider.GetRequiredService<MachinesJobService>();
        await schedulerService.UpdateMachinesAsync();
    }
}
