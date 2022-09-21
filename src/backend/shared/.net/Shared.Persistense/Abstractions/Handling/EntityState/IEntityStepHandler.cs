using Shared.Persistense.Abstractions.Entities.EntityState;

namespace Shared.Persistense.Abstractions.Handling.EntityState
{
    public interface IEntityStepHandler<in T> where T : class, IEntityState
    {
        Task HandleAsync(IEnumerable<T> entities, CancellationToken cToken);
    }
}