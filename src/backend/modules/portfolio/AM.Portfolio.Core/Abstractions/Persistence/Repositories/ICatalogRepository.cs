using AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;

namespace AM.Portfolio.Core.Abstractions.Persistence.Repositories;

public interface ICatalogRepository
{
    Task<EventType[]> GetEventTypes(CancellationToken cToken);
}
