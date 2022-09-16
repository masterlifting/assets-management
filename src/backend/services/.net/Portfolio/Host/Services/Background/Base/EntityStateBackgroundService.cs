using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AM.Services.Portfolio.Host.Services.Background.Base;

public abstract class EntityStateBackgroundService : BackgroundService
{
    private const string Action = "Отправка данных";
    private readonly Dictionary<string, OutgoingTaskSettings>? _tasks;

    private readonly ILogger _logger;
    private readonly IEntityStateBackgroundTask _task;

    protected EntityStateBackgroundService(IOptions<ServiceTaskSection> options, ILogger logger, IEntityStateBackgroundTask task)
    {
        var requests = options.Value.OutgoingTasks?.Requests;
        var responses = options.Value.OutgoingTasks?.Responses;
        var requestsCount = requests?.Count ?? 0;
        var responsesCount = responses?.Count ?? 0;

        _tasks = new Dictionary<string, OutgoingTaskSettings>(requestsCount + responsesCount);

        if (requestsCount > 0)
            foreach (var requestTask in requests!)
                _tasks.Add(requestTask.Key, requestTask.Value);

        if (responsesCount > 0)
            foreach (var responseTask in responses!)
                _tasks.Add(responseTask.Key, responseTask.Value);

        _task = task;
        _logger = logger;

    }
    protected override async Task ExecuteAsync(CancellationToken cToken)
    {
        if (_tasks is null || !_tasks.ContainsKey(_task.Name))
        {
            _logger.LogWarn(_task.Name, Action, "Конфигурация задачи не найдена");
            await StopAsync(cToken);
            return;
        }

        var settings = _tasks[_task.Name];

        if (!settings.Scheduler.IsReady(out var readyInfo))
        {
            _logger.LogWarn(_task.Name, Action, readyInfo);
            await StopAsync(cToken);
            return;
        }

        using var timer = new PeriodicTimer(TimeOnly.Parse(settings.Scheduler.WorkTime).ToTimeSpan());

        while (await timer.WaitForNextTickAsync(cToken))
        {
            if (settings.Scheduler.IsStop(out var stopInfo))
            {
                _logger.LogWarn(_task.Name, Action, stopInfo);
                await StopAsync(cToken);
                return;
            }

            if (!settings.Scheduler.IsStart(out var startInfo))
            {
                _logger.LogWarn(_task.Name, Action, startInfo);
                continue;
            }

            try
            {
                _logger.LogTrace(_task.Name, Action, AdapterConstants.Start);

                if (settings.Limit > AdapterConstants.MessagesLimit)
                {
                    settings.Limit = AdapterConstants.MessagesLimit;

                    _logger.LogWarn(_task.Name, Action, "Достигнут максимальный размер пакета", $"Значение конфигурации '{nameof(settings.Limit)}' установлено: {AdapterConstants.MessagesLimit}");
                }

                await _task.StartAsync(settings, cToken);

                _logger.LogTrace(_task.Name, Action, AdapterConstants.Done);
            }
            catch (Exception exception)
            {
                _logger.LogError(_task.Name, Action, exception);
            }
            finally
            {
                _logger.LogTrace(_task.Name, Action, $"Следующий запуск через {settings.Scheduler.WorkTime}");

                if (settings.Scheduler.IsOnce)
                    settings.Scheduler.SetOnce();
            }
        }
    }
    public override async Task StopAsync(CancellationToken cToken)
    {
        _logger.LogWarn(_task.Name, Action, "Остановлено");
        await base.StopAsync(cToken);
    }
}