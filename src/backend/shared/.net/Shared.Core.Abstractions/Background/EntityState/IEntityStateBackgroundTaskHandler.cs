using Shared.Contracts.Abstractions.Domains.Entities;
using Shared.Contracts.Settings;

namespace Shared.Core.Abstractions.Background.EntityState;

public interface IEntityStateBackgroundTaskHandler<TEntity, in TSettings> where TEntity : class, IEntityState where TSettings : BackgroundTaskSettings
{
    Task<long[]> GetProcessingIdsAsync(byte stepId, TSettings settings, CancellationToken cToken);
    Task<TEntity[]> GetEntitiesAsync(byte stepId, long[] ids, CancellationToken cToken);
    Task HandleEntitiesAsync(byte stepId, IEnumerable<TEntity> entities, CancellationToken cToken);
    Task UpdateEntitiesAsync(byte? nextStepId, IEnumerable<TEntity> entities, CancellationToken cToken);
}