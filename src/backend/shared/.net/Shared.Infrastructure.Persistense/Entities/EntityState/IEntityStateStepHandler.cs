namespace Shared.Infrastructure.Persistense.Entities.EntityState;

public interface IEntityStateStepHandler<in T> where T : class, IEntityState
{
    Task HandleAsync(IEnumerable<T> entities, CancellationToken cToken);
}