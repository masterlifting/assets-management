using Microsoft.Extensions.Logging;

using Shared.Background.Core.Base;
using Shared.Background.Core.Handlers;
using Shared.Background.Exceptions;
using Shared.Background.Settings;
using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;
using Shared.Persistense.Abstractions.Repositories;

using System.Collections.Concurrent;

using static Shared.Background.Constants;
using static Shared.Persistense.Abstractions.Constants.Enums;

namespace Shared.Background.Core.BackgroundTasks;

public sealed class SynchronousRequestsBackgroundTask<T> : BackgroundTaskBase where T : class, IEntityProcessable
{
    private readonly SemaphoreSlim _semaphore = new(1);

    private readonly ILogger<T> _logger;
    private readonly IEntityStateRepository<T> _entityStateRepository;
    private readonly BackgroundTaskStepHandler<T> _handler;

    public SynchronousRequestsBackgroundTask(
        ILogger<T> logger
        , IEntityStateRepository<T> entityStateRepository
        , ICatalogRepository<IProcessingStep> stepCatalogRepository
        , BackgroundTaskStepHandler<T> handler) : base(logger, stepCatalogRepository)
    {
        _logger = logger;
        _entityStateRepository = entityStateRepository;
        _handler = handler;
    }

    internal override async Task SuccessivelyHandleStepsAsync(Queue<IProcessingStep> steps, string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        for (var i = 0; i <= steps.Count; i++)
        {
            var step = steps.Dequeue();
            var action = step.Info ?? step.Name;

            var entities = await HandleDataAsync(step, taskName, action, settings.Steps.IsParallelProcessing, cToken);

            try
            {
                _logger.LogTrace(taskName, action + Actions.EntityStates.UpdateData, Actions.Start);


                await _entityStateRepository.SaveResultAsync(null, entities, cToken);

                _logger.LogDebug(taskName, action + Actions.EntityStates.UpdateData, Actions.Success);
            }
            catch (Exception exception)
            {
                _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.UpdateData, new(exception)));
            }
        }
    }
    internal override Task ParallelHandleStepsAsync(ConcurrentQueue<IProcessingStep> steps, string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        var tasks = Enumerable.Range(0, steps.Count).Select(x => ParallelHandleStepAsync(steps, taskName, taskCount, settings, cToken));
        return Task.WhenAll(tasks);
    }

    private async Task ParallelHandleStepAsync(ConcurrentQueue<IProcessingStep> steps, string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        var isDequeue = steps.TryDequeue(out var step);

        if (steps.Any() && !isDequeue)
            await ParallelHandleStepAsync(steps, taskName, taskCount, settings, cToken);

        var action = step!.Info ?? step.Name;

        var entities = await HandleDataAsync(step, taskName, action, settings.Steps.IsParallelProcessing, cToken);

        try
        {
            _logger.LogTrace(taskName, action + Actions.EntityStates.UpdateData, Actions.Start);

            await _semaphore.WaitAsync();
            await _entityStateRepository.SaveResultAsync(null, entities, cToken);
            _semaphore.Release();

            _logger.LogDebug(taskName, action + Actions.EntityStates.UpdateData, Actions.Success);
        }
        catch (Exception exception)
        {
            _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.UpdateData, new(exception)));
        }
    }
    private async Task<IReadOnlyCollection<T>> HandleDataAsync(IProcessingStep step, string taskName, string action, bool isParallel, CancellationToken cToken)
    {
        try
        {
            IReadOnlyCollection<T> entities;

            _logger.LogTrace(taskName, action + Actions.EntityStates.HandleData, Actions.Start);

            if (!isParallel)
                entities = await _handler.HandleAsync(step, cToken);
            else
            {
                await _semaphore.WaitAsync();
                entities = await _handler.HandleAsync(step, cToken);
                _semaphore.Release();
            }

            foreach (var entity in entities.Where(x => x.ProcessStatusId == (int)ProcessStatuses.Error))
                _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.HandleData, new(entity.Info ?? "Error has not description")));

            _logger.LogDebug(taskName, action + Actions.EntityStates.HandleData, Actions.Success);

            return entities;
        }
        catch (Exception exception)
        {
            throw new SharedBackgroundException(taskName, action + Actions.EntityStates.HandleData, new(exception));
        }
    }
}