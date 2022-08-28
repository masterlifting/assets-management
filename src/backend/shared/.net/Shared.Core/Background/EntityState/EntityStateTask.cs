using Shared.Contracts.Abstractions.Domains.Entities;
using Shared.Contracts.Settings;
using Shared.Core.Abstractions.Background.EntityState;

namespace Shared.Core.Background.EntityState;

public class EntityStateTask<T> where T : class, IEntityState
{
    public async Task StartAsync(string initiator, Queue<byte> steps, IEntityStateBackgroundTaskHandler<T, BackgroundTaskSettings> handler, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        for (var i = 0; i < steps.Count; i++)
        {
            var currentStepId = steps.Dequeue();
            var nextStepId = steps.Peek();

            var ids = await handler.GetProcessingIdsAsync(currentStepId, settings, cToken);

            if (!ids.Any())
                continue;

            var entities = await handler.GetEntitiesAsync(currentStepId, ids, cToken);

            await handler.HandleEntitiesAsync(currentStepId, entities, cToken);

            await handler.UpdateEntitiesAsync(nextStepId, entities, cToken);
        }
    }
}