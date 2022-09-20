using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using Shared.Persistense.Abstractions.Handling.EntityState;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Computing.Assets;

public sealed class AssetComputer : IEntityStepHandler<Asset>
{
    public Task HandleAsync(IEnumerable<Asset> entities, CancellationToken cToken)
    {
        throw new NotImplementedException();
    }
}