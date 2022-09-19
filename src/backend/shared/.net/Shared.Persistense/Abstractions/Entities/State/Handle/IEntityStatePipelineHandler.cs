namespace Shared.Persistense.Abstractions.Entities.State.Handle;

public interface IEntityStatePipelineHandler<in TEntity> where TEntity : class, IEntityState
{
    Task HandleDataAsync(IEntityStep step, IEnumerable<TEntity> data, CancellationToken cToken);
}