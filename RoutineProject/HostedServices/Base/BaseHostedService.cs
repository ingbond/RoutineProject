namespace RoutineProject.HostedServices.Base;

public abstract class BaseHostedService : IHostedService
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    protected readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly int _taskDebounceTimeoutMs;
    private readonly Thread _thread;
    private CancellationToken _token;

    protected BaseHostedService(
        ILogger logger,
        IServiceScopeFactory serviceScopeFactory,
        int taskDebounceTimeMs = 60 * 60 * 1000
    )
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _thread = new Thread(CyclicWorkSchedulerAsync);
        _taskDebounceTimeoutMs = taskDebounceTimeMs;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started");
        _thread.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopped");
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        return Task.CompletedTask;
    }

    protected abstract Task DoWorkAsync(IServiceScope scope, CancellationToken cancellationToken);

    private async void CyclicWorkSchedulerAsync()
    {
        _logger.LogInformation("Scheduler started");
        _token = _cancellationTokenSource.Token;

        while (!_token.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                _logger.LogDebug("Executing worker function");
                await DoWorkAsync(scope, _token);
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    _logger.LogError("Operation was canceled");
                    continue;
                }

                _logger.LogError(e, "Unhandled exception occured");
            }

            _token.WaitHandle.WaitOne(_taskDebounceTimeoutMs);
        }
    }
}
