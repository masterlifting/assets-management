using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.Contracts.Settings;
using Shared.Contracts.Settings.Sections;
using Shared.Core.Abstractions.Background.EntityState;
using Shared.Core.Constants;
using Shared.Core.Extensions;

namespace Shared.Core.Background.EntityState;

public abstract class EntityStateBackgroundService : BackgroundService
{
    private const string ActionName = "Фоновая задача";

    private PeriodicTimer _timer;
    private Dictionary<string, BackgroundTaskSettings>? _tasks;

    private readonly ILogger _logger;
    private readonly IEntityStateBackgroundTask<BackgroundTaskSettings> _task;


    protected EntityStateBackgroundService(IOptionsMonitor<BackgroundTaskSection> options, ILogger logger, IEntityStateBackgroundTask<BackgroundTaskSettings> task)
    {
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        _tasks = options.CurrentValue.Tasks;

        options.OnChange(x => _tasks = x.Tasks);

        _task = task;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cToken)
    {
        while (!cToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(cToken))
        {
            if (_tasks is null || !_tasks.ContainsKey(_task.Name))
            {
                _logger.LogWarn(_task.Name, ActionName, "Конфигурация задачи не найдена", "Следующая попытка через 10 минут");

                _timer.Dispose();
                _timer = new(TimeSpan.FromMinutes(10));

                continue;
            }

            var settings = _tasks[_task.Name];

            if (!settings.Scheduler.IsReady(out var readyResult))
            {
                if (!string.IsNullOrEmpty(readyResult))
                    _logger.LogDebug(_task.Name, ActionName, readyResult, $"Следующая попытка через {settings.Scheduler.RetryMinutes} минут");

                _timer.Dispose();
                _timer = new(TimeSpan.FromMinutes(settings.Scheduler.RetryMinutes));

                continue;
            }

            try
            {
                _logger.LogTrace(ActionName, _task.Name, CoreConstants.Results.Start);

                await _task.StartAsync(settings, cToken);

                _logger.LogInfo(ActionName, _task.Name, CoreConstants.Results.Stop);
            }
            catch (Exception exception)
            {
                _logger.LogError(ActionName, _task.Name, exception);
            }
            finally
            {
                _timer.Dispose();
                _timer = new(TimeOnly.Parse(settings.Scheduler.WorkTime).ToTimeSpan());

                _logger.LogDebug(_task.Name, ActionName, $"Следующий запуск через {settings.Scheduler.WorkTime}");
            }
        }
    }
    public override Task StopAsync(CancellationToken cToken)
    {
        _timer.Dispose();
        _logger.LogInfo(ActionName, _task.Name, CoreConstants.Results.Stop);

        return base.StopAsync(cToken);
    }
}