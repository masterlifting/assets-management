namespace Shared.Infrastructure.Persistense.Abstractions.Entities.State.Handle;

public interface IEntityStateStepHandler<in T> where T : class, IEntityState
{
    Task HandleAsync(IEnumerable<T> entities, CancellationToken cToken);
}