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

public sealed class EntitiesProcessingBackgroundTask<T> : BackgroundTaskBase where T : class, IEntityProcessable
{
    private readonly SemaphoreSlim _semaphore = new(1);

    private readonly ILogger<T> _logger;
    private readonly IEntityStateRepository<T> _entityStateRepository;
    private readonly BackgroundTaskStepHandler<T> _handler;

    public EntitiesProcessingBackgroundTask(
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

            Guid[]? ids = await GetPreparedIdsAsync(step, taskName, action, taskCount, settings, cToken);

            if (ids is null)
                continue;

            T[]? entities = await GetEntitiesAsync(step, taskName, action, ids, settings.Steps.IsParallelProcessing, cToken);

            if (entities is null)
                continue;

            await HandleDataAsync(step, taskName, action, entities, settings.Steps.IsParallelProcessing, cToken);

            var isNextStep = steps.TryPeek(out var nextStep);

            try
            {
                _logger.LogTrace(taskName, action + Actions.EntityStates.UpdateData, Actions.Start);

                if (isNextStep)
                {
                    foreach (var entity in entities.Where(x => x.ProcessStatusId == (int)ProcessStatuses.Processed))
                    {
                        entity.ProcessStatusId = (int)ProcessStatuses.Ready;
                        entity.Info = $"Previous step was '{step.Name}'";
                    }

                    await _entityStateRepository.SaveResultAsync(nextStep, entities, cToken);

                    _logger.LogDebug(taskName, action + Actions.EntityStates.UpdateData, Actions.Success, $"Next step: '{nextStep!.Name}' was set");
                }
                else
                {
                    await _entityStateRepository.SaveResultAsync(null, entities, cToken);

                    _logger.LogDebug(taskName, action + Actions.EntityStates.UpdateData, Actions.Success);
                }
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

        Guid[]? ids = await GetPreparedIdsAsync(step, taskName, action, taskCount, settings, cToken);

        if (ids is null)
            return;

        T[]? entities = await GetEntitiesAsync(step, taskName, action, ids, settings.Steps.IsParallelProcessing, cToken);

        if (entities is null)
            return;

        await HandleDataAsync(step, taskName, action, entities, settings.Steps.IsParallelProcessing, cToken);

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

    private async Task<Guid[]?> GetPreparedIdsAsync(IProcessingStep step, string taskName, string action, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        Guid[]? ids = null;
        try
        {
            _logger.LogTrace(taskName, action + Actions.EntityStates.PrepareNewData, Actions.Start);

            if (!settings.Steps.IsParallelProcessing)
                ids = await _entityStateRepository.PrepareDataAsync(step, settings.Steps.ProcessingMaxCount, cToken);
            else
            {
                await _semaphore.WaitAsync();
                ids = await _entityStateRepository.PrepareDataAsync(step, settings.Steps.ProcessingMaxCount, cToken);
                _semaphore.Release();
            }

            if (ids.Any())
                _logger.LogDebug(taskName, action + Actions.EntityStates.PrepareNewData, Actions.Success, ids.Length);

            if (taskCount % settings.RetryPolicy.EveryTime == 0)
            {
                _logger.LogTrace(taskName, action + Actions.EntityStates.PrepareUnhandledData, Actions.Start);

                var retryTime = TimeOnly.Parse(settings.Scheduler.WorkTime).ToTimeSpan() * settings.RetryPolicy.EveryTime;
                var retryDate = DateTime.UtcNow.Add(-retryTime);

                var retryIds = await _entityStateRepository.PrepareRetryDataAsync(step, settings.Steps.ProcessingMaxCount, retryDate, settings.RetryPolicy.MaxAttempts, cToken);

                if (retryIds.Any())
                {
                    ids = ids.Concat(retryIds).ToArray();
                    _logger.LogDebug(taskName, action + Actions.EntityStates.PrepareUnhandledData, Actions.Success, retryIds.Length);
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.PrepareData, new(exception)));
        }

        if (ids is not null && !ids.Any())
        {
            _logger.LogTrace(taskName, action + Actions.EntityStates.PrepareData, Actions.NoData);
        }

        return ids;
    }
    private async Task<T[]?> GetEntitiesAsync(IProcessingStep step, string taskName, string action, Guid[] ids, bool isParallel, CancellationToken cToken)
    {
        T[]? entities = null;
        try
        {
            _logger.LogTrace(taskName, action + Actions.EntityStates.GetData, Actions.Start);

            if (!isParallel)
                entities = await _entityStateRepository.GetDataAsync(step, ids, cToken);
            else
            {
                await _semaphore.WaitAsync();
                entities = await _entityStateRepository.GetDataAsync(step, ids, cToken);
                _semaphore.Release();
            }

            _logger.LogDebug(taskName, action + Actions.EntityStates.GetData, Actions.Success);
        }
        catch (Exception exception)
        {
            _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.GetData, new(exception)));
        }

        return entities;
    }
    private async Task HandleDataAsync(IProcessingStep step, string taskName, string action, T[] entities, bool isParallel, CancellationToken cToken)
    {
        try
        {
            _logger.LogTrace(taskName, action + Actions.EntityStates.HandleData, Actions.Start);

            if (!isParallel)
                await _handler.HandleAsync(step, entities, cToken);
            else
            {
                await _semaphore.WaitAsync();
                await _handler.HandleAsync(step, entities, cToken);
                _semaphore.Release();
            }

            foreach (var entity in entities.Where(x => x.ProcessStatusId != (int)ProcessStatuses.Error))
                entity.ProcessStatusId = (int)ProcessStatuses.Processed;

            foreach (var entity in entities.Where(x => x.ProcessStatusId == (int)ProcessStatuses.Error))
                _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.HandleData, new(entity.Info ?? "Error has not description")));

            _logger.LogDebug(taskName, action + Actions.EntityStates.HandleData, Actions.Success);
        }
        catch (Exception exception)
        {
            foreach (var entity in entities)
            {
                entity.ProcessStatusId = (int)ProcessStatuses.Error;
                entity.Info = exception.Message;
            }

            _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.HandleData, new(exception)));
        }
    }
}