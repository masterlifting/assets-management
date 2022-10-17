using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Shared.Persistense.Abstractions.Handling.EntityState;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Computing.Assets;

public sealed class AssetComputer : IEntityStepHandler<Asset>
{
    public Task HandleAsync(IEnumerable<Asset> entities, CancellationToken cToken)
    {
        throw new NotImplementedException();
    }
}