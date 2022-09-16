using Microsoft.Extensions.Logging;

using Shared.Contracts.Settings;
using Shared.Exceptions;
using Shared.Extensions.Logging;
using Shared.Infrastructure.Persistense.Enums;
using Shared.Infrastructure.Persistense.Repositories.Interface;

namespace Shared.Infrastructure.Persistense.Entities.EntityState;

public class EntityStatePipeline<TEntity> where TEntity : class, IEntityState
{
    private readonly ILogger<EntityStatePipeline<TEntity>> _logger;
    private readonly IEntityStateRepository<TEntity> _repository;
    private readonly IEntityStatePipelineHandler<TEntity> _handler;

    public EntityStatePipeline(
        ILogger<EntityStatePipeline<TEntity>> logger
        , IEntityStateRepository<TEntity> repository
        , IEntityStatePipelineHandler<TEntity> handler)
    {
        _logger = logger;
        _repository = repository;
        _handler = handler;
    }

    public async Task StartAsync(BackgroundTaskSettings settings, Queue<int> steps, CancellationToken cToken)
    {
        _logger.LogDebug("", "", "");

        for (var i = 0; i < steps.Count; i++)
        {
            var stepId = steps.Dequeue();

            string[] ids;
            try
            {
                if (settings.Retry is null)
                {
                    _logger.LogTrace("", "", "");
                    ids = await _repository.PrepareDataAsync(stepId, settings.Limit, cToken);
                    _logger.LogDebug("", "", "");
                }
                else
                {
                    var dateNow = DateTime.UtcNow;
                    var retryTime = TimeSpan.FromMinutes(settings.Retry!.Minutes);
                    var retryDate = dateNow.Add(-retryTime);

                    _logger.LogTrace("", "", "");
                    ids = await _repository.PrepareRetryDataAsync(stepId, settings.Limit, retryDate, settings.Retry.MaxAttempts, cToken);
                    _logger.LogDebug("", "", "");
                }
            }
            catch (Exception exception)
            {
                throw new SharedProcessException("", "", exception);
            }

            if (!ids.Any())
            {
                _logger.LogTrace("", "", "");
                continue;
            }

            TEntity[] entities;
            try
            {
                _logger.LogTrace("", "", "");
                entities = await _repository.GetDataAsync(stepId, ids, cToken);
                _logger.LogDebug("", "", "");
            }
            catch (Exception exception)
            {
                throw new SharedProcessException("", "", exception);
            }

            try
            {
                _logger.LogTrace("", "", "");
                await _handler.HandleDataAsync(stepId, entities, cToken);

                foreach (var entity in entities)
                    entity.StateId = (int)States.Processed;
                
                _logger.LogDebug("", "", "");
            }
            catch (Exception exception)
            {
                foreach (var entity in entities)
                {
                    entity.StateId = (int)States.Error;
                    entity.Info = exception.Message;
                }

                _logger.LogError("", "", exception);
            }

            var nextStepId = steps.Peek();
            try
            {
                _logger.LogTrace("", "", "");
                await _repository.SaveResultAsync(nextStepId, entities, cToken);
                _logger.LogDebug("", "", "");
            }
            catch (Exception exception)
            {
                throw new SharedProcessException("", "", exception);
            }
        }
    }
}