﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.Background.Settings;
using Shared.Background.Settings.Sections;
using Shared.Extensions.Logging;

namespace Shared.Background.Abstractions.EntityState;

public abstract class EntityStateBackgroundService : BackgroundService
{
    private const int Limit = 10_000;
    private const string Action = "Обработка сущностей с состоянием";
    private Dictionary<string, BackgroundTaskSettings>? _tasks;

    private readonly ILogger _logger;
    private readonly IEntityStateBackgroundTask _task;

    protected EntityStateBackgroundService(IOptionsMonitor<BackgroundTaskSection> options, ILogger logger, IEntityStateBackgroundTask task)
    {
        _tasks = options.CurrentValue.Tasks;
        options.OnChange(x => _tasks = x.Tasks);

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
                _logger.LogTrace(_task.Name, Action, "Запущено");

                if (settings.Limit > Limit)
                {
                    settings.Limit = Limit;

                    _logger.LogWarn(_task.Name, Action, "Достигнут максимальный размер пакета", $"Значение конфигурации '{nameof(settings.Limit)}' установлено: {Limit}");
                }

                await _task.StartAsync(settings, cToken);

                _logger.LogTrace(_task.Name, Action, "Выполнено");
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