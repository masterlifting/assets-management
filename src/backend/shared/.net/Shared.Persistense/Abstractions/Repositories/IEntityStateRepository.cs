using Shared.Persistense.Abstractions.Entities.EntityState;

namespace Shared.Persistense.Abstractions.Repositories;

public interface IEntityStateRepository<T> : IRepository<T> where T : class, IEntityState
{
    Task<string[]> PrepareDataAsync(IEntityStepType step, int limit, CancellationToken cToken);
    Task<string[]> PrepareRetryDataAsync(IEntityStepType step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken);
    Task<T[]> GetDataAsync(IEntityStepType step, IEnumerable<string> ids, CancellationToken cToken);
    Task SaveResultAsync(IEntityStepType? step, IEnumerable<T> entities, CancellationToken cToken);
}