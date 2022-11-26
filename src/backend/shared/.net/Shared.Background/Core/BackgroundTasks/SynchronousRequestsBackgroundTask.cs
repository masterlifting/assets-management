﻿using Microsoft.Extensions.Logging;

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

public abstract class SynchronousRequestsBackgroundTask<TEntity, TStep> : BackgroundTaskBase<TStep> 
    where TEntity : class, IProcessableEntity
    where TStep : class, IProcessableEntityStep
{
    private readonly SemaphoreSlim _semaphore = new(1);

    private readonly ILogger<TEntity> _logger;
    private readonly IRepository _repository;
    private readonly BackgroundTaskStepHandler<TEntity> _handler;

    public SynchronousRequestsBackgroundTask(
        ILogger<TEntity> logger
        , IRepository repository
        , BackgroundTaskStepHandler<TEntity> handler) : base(logger, repository)
    {
        _logger = logger;
        _repository = repository;
        _handler = handler;
    }

    internal override async Task SuccessivelyHandleStepsAsync(Queue<TStep> steps, string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        for (var i = 0; i <= steps.Count; i++)
        {
            var step = steps.Dequeue();
            var action = step.Info ?? step.Name;

            var entities = await HandleDataAsync(step, taskName, action, settings.Steps.IsParallelProcessing, cToken);

            try
            {
                _logger.LogTrace(taskName, action + Actions.EntityStates.UpdateData, Actions.Start);


                await _repository.SaveProcessableEntityResultAsync(null, entities, cToken);

                _logger.LogDebug(taskName, action + Actions.EntityStates.UpdateData, Actions.Success);
            }
            catch (Exception exception)
            {
                _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.UpdateData, new(exception)));
            }
        }
    }
    internal override Task ParallelHandleStepsAsync(ConcurrentQueue<TStep> steps, string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        var tasks = Enumerable.Range(0, steps.Count).Select(x => ParallelHandleStepAsync(steps, taskName, taskCount, settings, cToken));
        return Task.WhenAll(tasks);
    }

    private async Task ParallelHandleStepAsync(ConcurrentQueue<TStep> steps, string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken)
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
            await _repository.SaveProcessableEntityResultAsync(null, entities, cToken);
            _semaphore.Release();

            _logger.LogDebug(taskName, action + Actions.EntityStates.UpdateData, Actions.Success);
        }
        catch (Exception exception)
        {
            _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.UpdateData, new(exception)));
        }
    }
    private async Task<IReadOnlyCollection<TEntity>> HandleDataAsync(TStep step, string taskName, string action, bool isParallel, CancellationToken cToken)
    {
        try
        {
            IReadOnlyCollection<TEntity> entities;

            _logger.LogTrace(taskName, action + Actions.EntityStates.HandleData, Actions.Start);

            if (!isParallel)
                entities = await _handler.HandleAsync(step, cToken);
            else
            {
                await _semaphore.WaitAsync();
                entities = await _handler.HandleAsync(step, cToken);
                _semaphore.Release();
            }

            foreach (var entity in entities.Where(x => x.ProcessStatusId == (int)ProcessableEntityStatuses.Error))
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