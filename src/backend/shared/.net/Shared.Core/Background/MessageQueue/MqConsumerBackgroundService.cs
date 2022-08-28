using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.Core.Abstractions.Background.MessageQueue;
using Shared.Core.Abstractions.Queues;
using Shared.Core.Constants;
using Shared.Core.Extensions;

namespace Shared.Core.Background.MessageQueue;

public abstract class MqConsumerBackgroundService<TSettings> : BackgroundService where TSettings : class, IMqConsumerSettings
{
    private const string ActionName = "Фоновая задача менеджера очередей";

    private PeriodicTimer _timer;
    private Dictionary<string, TSettings>? _tasks;

    private readonly ILogger _logger;
    private readonly IMqConsumer _consumer;
    private readonly IMqConsumerBackgroundTask _task;

    protected MqConsumerBackgroundService(IOptionsMonitor<IMqConsumerSection<TSettings>> options, ILogger logger, IMqConsumer consumer, IMqConsumerBackgroundTask task)
    {
        _timer = new(TimeSpan.FromSeconds(5));
        _tasks = options.CurrentValue.Consumers;

        options.OnChange(x => _tasks = x.Consumers);

        _task = task;
        _logger = logger;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken cToken)
    {
        while (await _timer.WaitForNextTickAsync(cToken))
        {
            if (_tasks is null || !_tasks.ContainsKey(_task.Name))
            {
                _logger.LogWarn(ActionName, _task.Name, "Конфигурация задачи не найдена", "Следующая попытка через 10 минут");

                _consumer.Stop();

                _timer.Dispose();
                _timer = new(TimeSpan.FromMinutes(10));

                continue;
            }

            var settings = _tasks[_task.Name];

            if (!settings.Scheduler.IsReady(out var readyResult))
            {
                if (!string.IsNullOrEmpty(readyResult))
                    _logger.LogDebug(_task.Name, ActionName, readyResult, $"Следующая попытка через {settings.Scheduler.RetryMinutes} минут");

                _consumer.Stop();

                _timer.Dispose();
                _timer = new(TimeSpan.FromMinutes(settings.Scheduler.RetryMinutes));

                continue;
            }

            try
            {
                _logger.LogTrace(ActionName, _task.Name, CoreConstants.Results.Start);

                await _consumer.ConsumeAsync<string>(settings, _task.StartAsync, cToken);

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
        _consumer.Stop();
        _timer.Dispose();
        _logger.LogInfo(ActionName, _task.Name, CoreConstants.Results.Stop);
        
        return base.StopAsync(cToken);
    }
}