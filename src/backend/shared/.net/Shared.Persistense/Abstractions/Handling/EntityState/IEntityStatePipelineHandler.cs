using Shared.Persistense.Abstractions.Entities.EntityState;

namespace Shared.Persistense.Abstractions.Handling.EntityState;

public interface IEntityStatePipelineHandler<in TEntity> where TEntity : class, IEntityState
{
    Task HandleDataAsync(IEntityStepCatalog step, IEnumerable<TEntity> data, CancellationToken cToken);
}