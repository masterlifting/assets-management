using Shared.Persistense.Abstractions.Entities.State;

namespace Shared.Persistense.Abstractions.Repositories;

public interface IEntityStateRepository<T> : IRepository<T> where T : class, IEntityState
{
    Task<string[]> PrepareDataAsync(IEntityStep step, int limit, CancellationToken cToken);
    Task<string[]> PrepareRetryDataAsync(IEntityStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken);
    Task<T[]> GetDataAsync(IEntityStep step, IEnumerable<string> ids, CancellationToken cToken);
    Task SaveResultAsync(IEntityStep? step, IEnumerable<T> entities, CancellationToken cToken);
}