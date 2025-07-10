using AM.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;

using Net.Shared.Persistence.Abstractions.Repositories.NoSql;
using Net.Shared.Persistence.Abstractions.Repositories.Sql;
using Net.Shared.Persistence.Models.Contexts;

namespace AM.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class CatalogRepository : ICatalogRepository
{
    private readonly IPersistenceSqlReaderRepository _sqlReaderRepository;
    private readonly IPersistenceNoSqlReaderRepository _noSqlReaderRepository;

    public CatalogRepository(IPersistenceSqlReaderRepository sqlReaderRepository, IPersistenceNoSqlReaderRepository noSqlReaderRepository)
    {
        _sqlReaderRepository = sqlReaderRepository;
        _noSqlReaderRepository = noSqlReaderRepository;
    }

    public Task<EventType[]> GetEventTypes(CancellationToken cToken)
    {
        var options = new PersistenceQueryOptions<EventType>
        {
            Filter = x => true
        };

        return _sqlReaderRepository.FindMany(options, cToken);
    }
}
