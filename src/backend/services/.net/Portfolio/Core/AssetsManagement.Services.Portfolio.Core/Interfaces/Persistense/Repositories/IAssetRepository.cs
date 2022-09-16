using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Domain.Persistense.Models;
using Shared.Infrastructure.Persistense.Repositories.Interface;

namespace AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

public interface IAssetRepository : IEntityStateRepository<Asset>
{
    Task<Asset[]> GetNewAssetsAsync(IEnumerable<AssetModel> models);
}