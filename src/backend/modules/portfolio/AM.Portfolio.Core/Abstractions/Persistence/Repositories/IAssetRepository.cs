using AM.Portfolio.Core.Persistence.Entities.Sql;

namespace AM.Portfolio.Core.Abstractions.Persistence.Repositories;

public interface IAssetRepository
{
    Task Create(Asset asset, CancellationToken cToken);
    Task Create(IEnumerable<Asset> assets, CancellationToken cToken);

    Task<Asset[]> Get(CancellationToken cToken);
    Task<Asset[]> Get(int typeId, CancellationToken cToken);
    Task<Asset[]> Get(IEnumerable<int> typesId, CancellationToken cToken);
}
