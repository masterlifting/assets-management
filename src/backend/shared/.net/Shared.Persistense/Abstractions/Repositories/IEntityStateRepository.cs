using Shared.Persistense.Abstractions.Entities.EntityState;

namespace Shared.Persistense.Abstractions.Repositories;

public interface IEntityStateRepository<T> : IRepository<T> where T : class, IEntityState
{
    Task<string[]> PrepareDataAsync(IEntityStepCatalog step, int limit, CancellationToken cToken);
    Task<string[]> PrepareRetryDataAsync(IEntityStepCatalog step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken);
    Task<T[]> GetDataAsync(IEntityStepCatalog step, IEnumerable<string> ids, CancellationToken cToken);
    Task SaveResultAsync(IEntityStepCatalog? step, IEnumerable<T> entities, CancellationToken cToken);
}