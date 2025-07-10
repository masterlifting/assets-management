using AM.Services.Portfolio.Core.Domain.Persistence.Entities;

using Shared.Persistence.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories
{
    public interface IDealRepository : IPersistenceSqlRepository<Deal>
    {
    }
}
