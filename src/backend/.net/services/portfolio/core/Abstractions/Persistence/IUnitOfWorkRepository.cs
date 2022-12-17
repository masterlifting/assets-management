using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistence
{
    public interface IUnitOfWorkRepository
    {
        public IPersistenceNoSqlRepository<IncomingData> IncomingData { get; }
        public IPersistenceSqlRepository<ProcessStep> ProcessStep { get; }
        public IPersistenceSqlRepository<Asset> Asset { get; }
        public IPersistenceSqlRepository<Deal> Deal { get; }
        public IPersistenceSqlRepository<Event> Event { get; }
        public IPersistenceSqlRepository<Derivative> Derivative { get; }
        public IPersistenceSqlRepository<User> User { get; }
    }
}
