using RoutineProject.HostedServices.Base;
using RoutineProject.Services;

namespace RoutineProject.HostedServices;

public class MachinesHostedService(ILogger<MachinesHostedService> logger, IServiceScopeFactory serviceScopeFactory) : BaseHostedService(logger, serviceScopeFactory, _taskDebounceTimeoutMs)
{
    private const int _taskDebounceTimeoutMs = 10 * 60000; // every 10 min

    protected override async Task DoWorkAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var schedulerService = scope.ServiceProvider.GetRequiredService<MachinesJobService>();
        await schedulerService.UpdateMachinesAsync();
    }
}
