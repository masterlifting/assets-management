using Shared.Persistense.Abstractions.Entities.EntityState;

namespace Shared.Persistense.Abstractions.Handling.EntityState;

public interface IEntityStateHandler<in TEntity> where TEntity : class, IEntityState
{
    Task HandleDataAsync(IEntityStepType step, IEnumerable<TEntity> data, CancellationToken cToken);
}