using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using Shared.Infrastructure.Persistense.Entities.EntityState;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Calculating.Assets;

public class AssetCalculator : IEntityStateStepHandler<Asset>
{
    public Task HandleAsync(IEnumerable<Asset> entities, CancellationToken cToken)
    {
        throw new NotImplementedException();
    }
}