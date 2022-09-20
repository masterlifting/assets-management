using Microsoft.Extensions.Logging;

using Shared.Background.Settings;
using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;
using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Entities;
using Shared.Persistense.Enums;
using Shared.Persistense.Exceptions;

namespace Shared.Persistense.Handling.EntityState;

public sealed class EntityStatePipeline<TEntity> where TEntity : class, IEntityState
{
    private const string Start = "Запущено";
    private const string Success = "Успешно";
    private readonly string _initiator = $"{typeof(TEntity).Name} state";

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
                _logger.LogTrace(_initiator, action + ". Подготовка новых данных", Start);
                ids = await _repository.PrepareDataAsync(step, settings.Limit, cToken);

                if ((decimal)(count % settings.Retry.Skip) == 0)
                {
                    _logger.LogTrace(_initiator, action + ". Подготовка необработанных данных", Start);

                    var retryTime = TimeOnly.Parse(settings.Scheduler.WorkTime).ToTimeSpan() * 3;
                    var retryDate = DateTime.UtcNow.Add(-retryTime);
                    var retryIds = await _repository.PrepareRetryDataAsync(step, settings.Limit, retryDate, settings.Retry.Attempts, cToken);

                    ids = ids.Concat(retryIds).ToArray();
                }
            }
            catch (Exception exception)
            {
                throw new SharedPersistenseEntityStateException(_initiator, action, exception);
            }

            if (!ids.Any())
            {
                _logger.LogTrace(_initiator, action + ". Подготовка данных", "Не найдено");
                continue;
            }

            _logger.LogDebug(_initiator, action + ". Подготовка данных", Success, ids.Length);

            TEntity[] entities;
            try
            {
                _logger.LogTrace(_initiator, action + ". Получение данных", Start);
                entities = await _repository.GetDataAsync(step, ids, cToken);
                _logger.LogDebug(_initiator, action + ". Получение данных", Success);
            }
            catch (Exception exception)
            {
                throw new SharedPersistenseEntityStateException(_initiator, action, exception);
            }

            try
            {
                _logger.LogTrace(_initiator, action + ". Обработка данных", Start);
                await _handler.HandleDataAsync(step, entities, cToken);

                foreach (var entity in entities)
                    entity.StateId = (int)States.Processed;

                _logger.LogDebug(_initiator, action + ". Обработка данных", Success);
            }
            catch (Exception exception)
            {
                foreach (var entity in entities)
                {
                    entity.StateId = (int)States.Error;
                    entity.Info = exception.Message;
                }

                _logger.LogError(new SharedPersistenseEntityStateException(_initiator, action + ". Обработка данных", exception));
            }

            var nextStep = steps.Peek();
            try
            {
                _logger.LogTrace(_initiator, action + ". Обновление данных", Start);
                await _repository.SaveResultAsync(nextStep, entities, cToken);
                _logger.LogDebug(_initiator, action + ". Обновление данных", Success);
            }
            catch (Exception exception)
            {
                throw new SharedPersistenseEntityStateException(_initiator, action + ". Обновление данных", exception);
            }
        }
    }
}