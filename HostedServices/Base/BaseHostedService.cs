namespace RoutineProject.HostedServices.Base;

public abstract class BaseHostedService : BackgroundService
{
    protected readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _interval;

    protected BaseHostedService(
        ILogger logger,
        IServiceScopeFactory scopeFactory,
        TimeSpan interval)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _interval = interval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scheduler started");

        using var timer = new PeriodicTimer(_interval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    _logger.LogDebug("Executing worker function");

                    await DoWorkAsync(scope, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Execution canceled");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception occurred");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
            _logger.LogInformation("Scheduler shutdown");
        }

        _logger.LogInformation("Scheduler stopped");
    }

    protected abstract Task DoWorkAsync(
        IServiceScope scope,
        CancellationToken cancellationToken);
}
