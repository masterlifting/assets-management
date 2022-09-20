using Microsoft.Extensions.Logging;

using Shared.Background.Settings;
using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;
using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Entities;
using Shared.Persistense.Exceptions;

using static Shared.Persistense.Constants.Enums;

namespace Shared.Persistense.Handling.EntityState;

public sealed class EntityStateHandler<TEntity> where TEntity : class, IEntityState
{
    private readonly string _initiator = $"{typeof(TEntity).Name} state";

    private readonly ILogger<EntityStateHandler<TEntity>> _logger;
    private readonly IEntityStateRepository<TEntity> _repository;
    private readonly IEntityStatePipelineHandler<TEntity> _handler;

    public EntityStateHandler(
        ILogger<EntityStateHandler<TEntity>> logger
        , IEntityStateRepository<TEntity> repository
        , IEntityStatePipelineHandler<TEntity> handler)
    {
        _logger = logger;
        _repository = repository;
        _handler = handler;
    }

    public async Task StartAsync<TStep>(int count, BackgroundTaskSettings settings, Queue<TStep> steps, CancellationToken cToken)
        where TStep : Catalog, IEntityStepCatalog
    {
        _logger.LogDebug(_initiator, "Запуск шагов обработки", steps.Count);

        for (var i = 0; i < steps.Count; i++)
        {
            var step = steps.Dequeue();
            var action = step.Description ?? step.Name;

            string[] ids;
            try
            {
                _logger.LogTrace(_initiator, action + Constants.Actions.EntityStates.PrepareNewData, Constants.Actions.Start);
                ids = await _repository.PrepareDataAsync(step, settings.Limit, cToken);

                if ((decimal)(count % settings.Retry.Skip) == 0)
                {
                    _logger.LogTrace(_initiator, action + Constants.Actions.EntityStates.PrepareUnhandledData, Constants.Actions.Start);

                    var retryTime = TimeOnly.Parse(settings.Scheduler.WorkTime).ToTimeSpan() * settings.Retry.Skip;
                    var retryDate = DateTime.UtcNow.Add(-retryTime);
                    var retryIds = await _repository.PrepareRetryDataAsync(step, settings.Limit, retryDate, settings.Retry.Attempts, cToken);

                    ids = ids.Concat(retryIds).ToArray();
                }
            }
            catch (Exception exception)
            {
                throw new SharedPersistenseEntityStateException(_initiator, action + Constants.Actions.EntityStates.PrepareData, exception);
            }

            if (!ids.Any())
            {
                _logger.LogTrace(_initiator, action + Constants.Actions.EntityStates.PrepareData, Constants.Actions.NoData);
                continue;
            }

            _logger.LogDebug(_initiator, action + Constants.Actions.EntityStates.PrepareData, Constants.Actions.Success, ids.Length);

            TEntity[] entities;
            try
            {
                _logger.LogTrace(_initiator, action + Constants.Actions.EntityStates.GetData, Constants.Actions.Start);
                entities = await _repository.GetDataAsync(step, ids, cToken);
                _logger.LogDebug(_initiator, action + Constants.Actions.EntityStates.GetData, Constants.Actions.Success);
            }
            catch (Exception exception)
            {
                throw new SharedPersistenseEntityStateException(_initiator, action + Constants.Actions.EntityStates.GetData, exception);
            }

            try
            {
                _logger.LogTrace(_initiator, action + Constants.Actions.EntityStates.HandleData, Constants.Actions.Start);
                await _handler.HandleDataAsync(step, entities, cToken);

                foreach (var entity in entities)
                    entity.StateId = (int)States.Processed;

                _logger.LogDebug(_initiator, action + Constants.Actions.EntityStates.HandleData, Constants.Actions.Success);
            }
            catch (Exception exception)
            {
                foreach (var entity in entities)
                {
                    entity.StateId = (int)States.Error;
                    entity.Info = exception.Message;
                }

                _logger.LogError(new SharedPersistenseEntityStateException(_initiator, action + Constants.Actions.EntityStates.HandleData, exception));
            }

            var nextStep = steps.Peek();
            try
            {
                _logger.LogTrace(_initiator, action + Constants.Actions.EntityStates.UpdateData, Constants.Actions.Start);
                await _repository.SaveResultAsync(nextStep, entities, cToken);
                _logger.LogDebug(_initiator, action + Constants.Actions.EntityStates.UpdateData, Constants.Actions.Success);
            }
            catch (Exception exception)
            {
                throw new SharedPersistenseEntityStateException(_initiator, action + Constants.Actions.EntityStates.UpdateData, exception);
            }
        }
    }
}