using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace Shared.Persistense.Abstractions.Repositories;

public interface IEntityStateRepository<T> : IEntityRepository<T> where T : class, IEntityProcessable
{
    Task<Guid[]> PrepareDataAsync(IProcessingStep step, int limit, CancellationToken cToken);
    Task<Guid[]> PrepareRetryDataAsync(IProcessingStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken);
    Task<T[]> GetDataAsync(IProcessingStep step, IEnumerable<Guid> ids, CancellationToken cToken);
    Task SaveResultAsync(IProcessingStep? step, IEnumerable<T> entities, CancellationToken cToken);
}