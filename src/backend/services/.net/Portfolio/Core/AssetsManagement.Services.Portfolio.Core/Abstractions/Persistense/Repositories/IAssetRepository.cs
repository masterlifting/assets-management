using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Domain.Persistense.Models;

using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;

public interface IAssetRepository : IEntityStateRepository<Asset>
{
    Task<Asset[]> GetNewAssetsAsync(IEnumerable<AssetModel> models);
}