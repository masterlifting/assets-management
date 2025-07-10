using AM.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Portfolio.Core.Persistence.Entities.Sql;
using Net.Shared.Persistence.Abstractions.Repositories.Sql;
using Net.Shared.Persistence.Models.Contexts;

namespace AM.Portfolio.Infrastructure.Persistence.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly IPersistenceSqlReaderRepository _readerRepository;
    private readonly IPersistenceSqlWriterRepository _writerRepository;

    public AssetRepository(IPersistenceSqlReaderRepository readerRepository, IPersistenceSqlWriterRepository writerRepository)
    {
        _readerRepository = readerRepository;
        _writerRepository = writerRepository;
    }

    public Task Create(Asset asset, CancellationToken cToken)
    {
        return _writerRepository.CreateOne(asset, cToken);
    }

    public Task Create(IEnumerable<Asset> assets, CancellationToken cToken)
    {
        return _writerRepository.CreateMany(assets.ToArray(), cToken);
    }

    public Task<Asset[]> Get(CancellationToken cToken)
    {
        var options = new PersistenceQueryOptions<Asset>
        {
            Filter = x => true
        };

        return _readerRepository.FindMany(options, cToken);
    }

    public Task<Asset[]> Get(int typeId, CancellationToken cToken)
    {
        var options = new PersistenceQueryOptions<Asset>
        {
            Filter = x => x.TypeId == typeId
        };

        return _readerRepository.FindMany(options, cToken);
    }

    public Task<Asset[]> Get(IEnumerable<int> typesId, CancellationToken cToken)
    {
        var options = new PersistenceQueryOptions<Asset>
        {
            Filter = x => typesId.Contains(x.TypeId)
        };

        return _readerRepository.FindMany(options, cToken);
    }
}
