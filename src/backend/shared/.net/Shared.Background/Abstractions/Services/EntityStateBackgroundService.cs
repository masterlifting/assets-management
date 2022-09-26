using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Background.Abstractions.Tasks;
using Shared.Background.Exceptions;
using Shared.Background.Settings;
using Shared.Background.Settings.Sections;
using Shared.Extensions.Logging;

namespace Shared.Background.Abstractions.Services
{
    public abstract class EntityStateBackgroundService : BackgroundService
    {
        private int _count;
        private const int Limit = 5_000;
        private const string Action = "Обработка объектов с состоянием";
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
                _logger.LogWarn(_task.Name, Action, Constants.Actions.NoConfig);
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

            var timerPeriod = TimeOnly.Parse(settings.Scheduler.WorkTime).ToTimeSpan();
            using var timer = new PeriodicTimer(timerPeriod);

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
                    if (_count == int.MaxValue)
                        _count = 0;

                    _count++;

                    _logger.LogTrace(_task.Name, Action, Constants.Actions.Start);

                    if (settings.Limit > Limit)
                    {
                        settings.Limit = Limit;

                        _logger.LogWarn(_task.Name, Action, Constants.Actions.Limit, Limit);
                    }

                    await _task.StartAsync(_count, settings, cToken);

                    _logger.LogTrace(_task.Name, Action, Constants.Actions.Done);
                }
                catch (Exception exception)
                {
                    _logger.LogError(new SharedBackgroundException(_task.Name, Action, exception));
                }
                finally
                {
                    _logger.LogTrace(_task.Name, Action, Constants.Actions.NextStart + settings.Scheduler.WorkTime);

                    if (settings.Scheduler.IsOnce)
                        settings.Scheduler.SetOnce();
                }
            }
        }
        public override async Task StopAsync(CancellationToken cToken)
        {
            _logger.LogWarn(_task.Name, Action, Constants.Actions.Stop);
            await base.StopAsync(cToken);
        }
    }
}