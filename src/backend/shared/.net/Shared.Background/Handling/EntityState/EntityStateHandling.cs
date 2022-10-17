using Microsoft.Extensions.Logging;

using Shared.Background.Exceptions;
using Shared.Background.Settings;
using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;
using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Entities.EntityState;
using Shared.Persistense.Models.ValueObject;
using Shared.Persistense.Models.ValueObject.EntityState;

using static Shared.Background.Constants;
using static Shared.Persistense.Constants.Enums;

namespace Shared.Background.Handling.EntityState;

public sealed class EntityStateHandling<TEntity, TRepository>
    where TEntity : class, IEntityState
    where TRepository : IEntityStateRepository<TEntity>
{
    private readonly string _initiator = $"Handler {typeof(TEntity).Name}";

    private readonly ILogger<TEntity> _logger;
    private readonly TRepository _repository;
    private readonly IEntityStateHandler<TEntity> _handler;

    public EntityStateHandling(ILogger<TEntity> logger, TRepository repository, IEntityStateHandler<TEntity> handler)
    {
        _logger = logger;
        _repository = repository;
        _handler = handler;
    }

    public async Task StartAsync(int count, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        var steps = GetQueueSteps(settings.Steps);

        _logger.LogTrace(_initiator, "Begin processing by steps", Actions.Start, "Steps count: " + steps.Count);
        
        for (var i = 0; i <= steps.Count; i++)
        {
            var step = steps.Dequeue();
            var action = step.Info ?? step.Name;

            string[] ids;
            try
            {
                _logger.LogTrace(_initiator, action + Actions.EntityStates.PrepareNewData, Actions.Start);
                ids = await _repository.PrepareDataAsync(step, settings.Limit, cToken);

                if (ids.Any())
                    _logger.LogDebug(_initiator, action + Actions.EntityStates.PrepareNewData, Actions.Success, ids.Length);

                if (count % settings.Retry.Skip == 0)
                {
                    _logger.LogTrace(_initiator, action + Actions.EntityStates.PrepareUnhandledData, Actions.Start);

                    var retryTime = TimeOnly.Parse(settings.Scheduler.WorkTime).ToTimeSpan() * settings.Retry.Skip;
                    var retryDate = DateTime.UtcNow.Add(-retryTime);

                    var retryIds = await _repository.PrepareRetryDataAsync(step, settings.Limit, retryDate, settings.Retry.Attempts, cToken);

                    if (retryIds.Any())
                    {
                        ids = ids.Concat(retryIds).ToArray();
                        _logger.LogDebug(_initiator, action + Actions.EntityStates.PrepareUnhandledData, Actions.Success, retryIds.Length);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(new SharedBackgroundException(_initiator, action + Actions.EntityStates.PrepareData, exception));
                continue;
            }

            if (!ids.Any())
            {
                _logger.LogTrace(_initiator, action + Actions.EntityStates.PrepareData, Actions.NoData);
                continue;
            }

            TEntity[] entities;
            try
            {
                _logger.LogTrace(_initiator, action + Actions.EntityStates.GetData, Actions.Start);
                entities = await _repository.GetDataAsync(step, ids, cToken);
                _logger.LogDebug(_initiator, action + Actions.EntityStates.GetData, Actions.Success);
            }
            catch (Exception exception)
            {
                _logger.LogError(new SharedBackgroundException(_initiator, action + Actions.EntityStates.GetData, exception));
                continue;
            }

            try
            {
                _logger.LogTrace(_initiator, action + Actions.EntityStates.HandleData, Actions.Start);
                await _handler.HandleDataAsync(step, entities, cToken);

                foreach (var entity in entities.Where(x => x.StateId != (int)States.Error))
                    entity.StateId = (int)States.Processed;

                _logger.LogDebug(_initiator, action + Actions.EntityStates.HandleData, Actions.Success);
            }
            catch (Exception exception)
            {
                foreach (var entity in entities)
                {
                    entity.StateId = (int)States.Error;
                    entity.Info = exception.Message;
                }

                _logger.LogError(new SharedBackgroundException(_initiator, action + Actions.EntityStates.HandleData, exception));
            }

            var isNextStep = steps.TryPeek(out var nextStep);

            try
            {
                _logger.LogTrace(_initiator, action + Actions.EntityStates.UpdateData, Actions.Start);

                if (isNextStep)
                {
                    foreach (var entity in entities.Where(x => x.StateId == (int)States.Processed))
                        entity.StateId = (int)States.Ready;

                    await _repository.SaveResultAsync(nextStep, entities, cToken);
                    _logger.LogDebug(_initiator, action + Actions.EntityStates.UpdateData, Actions.Success, $"Next step: '{nextStep!.Name}' is set");
                }
                else
                {
                    await _repository.SaveResultAsync(null, entities, cToken);
                    _logger.LogDebug(_initiator, action + Actions.EntityStates.UpdateData, Actions.Success);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(new SharedBackgroundException(_initiator, action + Actions.EntityStates.UpdateData, exception));
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