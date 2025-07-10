using AM.Services.Portfolio.Core.Domain.Persistence.Collections;

using Shared.Persistence.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories
{
    public interface IIncomingDataRepository : IPersistenceNoSqlRepository<IncomingData>
    {
    }
}
