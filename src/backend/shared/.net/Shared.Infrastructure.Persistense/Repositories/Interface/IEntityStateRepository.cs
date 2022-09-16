﻿using Shared.Infrastructure.Persistense.Entities.EntityState;

namespace Shared.Infrastructure.Persistense.Repositories.Interface;

public interface IEntityStateRepository<T> : IRepository<T> where T : class, IEntityState
{
    Task<string[]> PrepareDataAsync(int stepId, int limit, CancellationToken cToken);
    Task<string[]> PrepareRetryDataAsync(int stepId, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken);
    Task<T[]> GetDataAsync(int stepId, IEnumerable<string> ids, CancellationToken cToken);
    Task SaveResultAsync(int? stepId, IEnumerable<T> entities, CancellationToken cToken);
}