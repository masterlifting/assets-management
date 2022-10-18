using Shared.Persistense.Abstractions.Entities.EntityState;

namespace Shared.Background.Abstractions.EntityState;

public interface IEntityStepHandler<in T> where T : class, IEntityState
{
    Task HandleAsync(IEnumerable<T> entities, CancellationToken cToken);
}