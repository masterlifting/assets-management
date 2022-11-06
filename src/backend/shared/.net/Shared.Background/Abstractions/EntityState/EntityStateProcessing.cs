using Microsoft.Extensions.Logging;
using Shared.Background.Exceptions;
using Shared.Background.Settings;
using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Entities.EntityState;
using Shared.Persistense.Models.ValueObject.EntityState;

using System;

using static Shared.Background.Constants;
using static Shared.Persistense.Constants.Enums;

namespace Shared.Background.Abstractions.EntityState;

public sealed class EntityStateProcessing<TEntity> where TEntity : class, IEntityState
{
    private readonly ILogger<TEntity> _logger;
    private readonly EntityStateHandler<TEntity> _handler;

    public EntityStateProcessing(ILogger<TEntity> logger, EntityStateHandler<TEntity> handler)
    {
        _logger = logger;
        _handler = handler;
    }

    public async Task StartAsync(string taskName, int count, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        var steps = GetQueueSteps(settings.Steps);

        _logger.LogTrace(taskName, "Begin processing by steps", Actions.Start, "Steps count: " + steps.Count);

        for (var i = 0; i <= steps.Count; i++)
        {
            var step = steps.Dequeue();
            var action = step.Info ?? step.Name;

            string[] ids;
            try
            {
                _logger.LogTrace(taskName, action + Actions.EntityStates.PrepareNewData, Actions.Start);

                ids = await _handler.Repository.PrepareDataAsync(step, settings.Limit, cToken);

                if (ids.Any())
                    _logger.LogDebug(taskName, action + Actions.EntityStates.PrepareNewData, Actions.Success, ids.Length);

                if (count % settings.Retry.Skip == 0)
                {
                    _logger.LogTrace(taskName, action + Actions.EntityStates.PrepareUnhandledData, Actions.Start);

                    var retryTime = TimeOnly.Parse(settings.Scheduler.WorkTime).ToTimeSpan() * settings.Retry.Skip;
                    var retryDate = DateTime.UtcNow.Add(-retryTime);

                    var retryIds = await _handler.Repository.PrepareRetryDataAsync(step, settings.Limit, retryDate, settings.Retry.Attempts, cToken);

                    if (retryIds.Any())
                    {
                        ids = ids.Concat(retryIds).ToArray();
                        _logger.LogDebug(taskName, action + Actions.EntityStates.PrepareUnhandledData, Actions.Success, retryIds.Length);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.PrepareData, exception));
                continue;
            }

            if (!ids.Any())
            {
                _logger.LogTrace(taskName, action + Actions.EntityStates.PrepareData, Actions.NoData);
                continue;
            }

            TEntity[] entities;
            try
            {
                _logger.LogTrace(taskName, action + Actions.EntityStates.GetData, Actions.Start);

                entities = await _handler.Repository.GetDataAsync(step, ids, cToken);
                
                _logger.LogDebug(taskName, action + Actions.EntityStates.GetData, Actions.Success);
            }
            catch (Exception exception)
            {
                _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.GetData, exception));
                continue;
            }

            try
            {
                _logger.LogTrace(taskName, action + Actions.EntityStates.HandleData, Actions.Start);

                await _handler.HandleDataAsync(step, entities, cToken);

                foreach (var entity in entities.Where(x => x.StateId != (int)States.Error))
                    entity.StateId = (int)States.Processed;

                foreach (var entity in entities.Where(x => x.StateId == (int)States.Error))
                    _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.HandleData, entity.Info ?? "Error has not description"));

                _logger.LogDebug(taskName, action + Actions.EntityStates.HandleData, Actions.Success);
            }
            catch (Exception exception)
            {
                foreach (var entity in entities)
                {
                    entity.StateId = (int)States.Error;
                    entity.Info = exception.Message;
                }

                _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.HandleData, exception));
            }

            var isNextStep = steps.TryPeek(out var nextStep);

            try
            {
                _logger.LogTrace(taskName, action + Actions.EntityStates.UpdateData, Actions.Start);

                if (isNextStep)
                {
                    foreach (var entity in entities.Where(x => x.StateId == (int)States.Processed))
                    {
                        entity.StateId = (int)States.Ready;
                        entity.Info = $"Previous step was '{step.Name}'";
                    }

                    await _handler.Repository.SaveResultAsync(nextStep, entities, cToken);

                    _logger.LogDebug(taskName, action + Actions.EntityStates.UpdateData, Actions.Success, $"Next step: '{nextStep!.Name}' was set");
                }
                else
                {
                    await _handler.Repository.SaveResultAsync(null, entities, cToken);

                    _logger.LogDebug(taskName, action + Actions.EntityStates.UpdateData, Actions.Success);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(new SharedBackgroundException(taskName, action + Actions.EntityStates.UpdateData, exception));
            }
        }
    }
    private static Queue<Step> GetQueueSteps(Steps[] steps)
    {
        var result = new Queue<Step>(steps.Length);

        foreach (var step in steps)
            result.Enqueue(new Step(new StepId(step)));

        return result;
    }
}