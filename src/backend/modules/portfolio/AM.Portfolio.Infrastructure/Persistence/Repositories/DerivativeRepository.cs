using AM.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Portfolio.Core.Persistence.Entities.Sql;

using Microsoft.EntityFrameworkCore;

using Net.Shared.Persistence.Abstractions.Repositories.Sql;
using Net.Shared.Persistence.Models.Contexts;

namespace AM.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class DerivativeRepository : IDerivativeRepository
{
    private readonly IPersistenceSqlReaderRepository _readerRepository;
    private readonly IPersistenceSqlWriterRepository _writerRepository;

    public DerivativeRepository(IPersistenceSqlReaderRepository readerRepository, IPersistenceSqlWriterRepository writerRepository)
    {
        _readerRepository = readerRepository;
        _writerRepository = writerRepository;
    }

    public Task Create(Derivative derivative, CancellationToken cToken)
    {
        return _writerRepository.CreateOne(derivative, cToken);
    }

    public Task Create(IEnumerable<Derivative> derivatives, CancellationToken cToken)
    {
        return _writerRepository.CreateMany(derivatives.ToArray(), cToken);
    }

    public Task<Derivative[]> Get(CancellationToken cToken)
    {
        var options = new PersistenceQueryOptions<Derivative>
        {
            Filter = x => true
        };

        return _readerRepository.FindMany(options, cToken);
    }

    public Task<Derivative[]> Get(int assetId, CancellationToken cToken)
    {
        var options = new PersistenceQueryOptions<Derivative>
        {
            Filter = x => x.AssetId == assetId
        };

        return _readerRepository.FindMany(options, cToken);
    }

    public Task<Derivative[]> Get(IEnumerable<int> assetsId, CancellationToken cToken)
    {
        var options = new PersistenceQueryOptions<Derivative>
        {
            Filter = x => assetsId.Contains(x.AssetId)
        };

        return _readerRepository.FindMany(options, cToken);
    }
}
