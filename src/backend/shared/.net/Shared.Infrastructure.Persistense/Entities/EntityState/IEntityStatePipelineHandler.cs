namespace Shared.Infrastructure.Persistense.Entities.EntityState;

public interface IEntityStatePipelineHandler<in TEntity> where TEntity : class, IEntityState
{
    Task HandleDataAsync(int stepId, IEnumerable<TEntity> data, CancellationToken cToken);
}