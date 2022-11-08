using Shared.Background.Exceptions;
using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Repositories;

namespace Shared.Background.Abstractions.EntityState;

public abstract class EntityStateHandler<TEntity> where TEntity : class, IEntityState
{
    public IEntityStateRepository<TEntity> Repository { get; }

    private readonly Dictionary<int, IEntityStepHandler<TEntity>> _handlers;

    public EntityStateHandler(IEntityStateRepository<TEntity> repository, Dictionary<int, IEntityStepHandler<TEntity>> handlers)
    {
        Repository = repository;
        _handlers = handlers;
    }

    public Task HandleDataAsync(IEntityStepType step, IEnumerable<TEntity> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleStepAsync(data, cToken)
        : throw new SharedBackgroundException(typeof(TEntity).Name + "Handler", nameof(HandleDataAsync), new($"Step: '{step.Name}' not implemented'"));
}