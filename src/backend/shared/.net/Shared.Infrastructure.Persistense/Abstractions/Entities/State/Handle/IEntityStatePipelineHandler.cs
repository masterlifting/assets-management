namespace Shared.Infrastructure.Persistense.Abstractions.Entities.State.Handle;

public interface IEntityStatePipelineHandler<in TEntity> where TEntity : class, IEntityState
{
    Task HandleDataAsync(int stepId, IEnumerable<TEntity> data, CancellationToken cToken);
}