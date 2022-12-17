using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        public UnitOfWorkRepository(
            IPersistenceNoSqlRepository<IncomingData> incomingData
            , IPersistenceSqlRepository<ProcessStep> processStep
            , IPersistenceSqlRepository<Asset> asset
            , IPersistenceSqlRepository<Deal> deal
            , IPersistenceSqlRepository<Event> _event
            , IPersistenceSqlRepository<Derivative> derivative
            , IPersistenceSqlRepository<User> user)
        {
            IncomingData = incomingData;
            ProcessStep = processStep;
            Asset = asset;
            Deal = deal;
            Event = _event;
            Derivative = derivative;
            User = user;
        }

        public IPersistenceNoSqlRepository<IncomingData> IncomingData { get; }
        public IPersistenceSqlRepository<ProcessStep> ProcessStep { get; }
        public IPersistenceSqlRepository<Asset> Asset { get; }
        public IPersistenceSqlRepository<Deal> Deal { get; }
        public IPersistenceSqlRepository<Event> Event { get; }
        public IPersistenceSqlRepository<Derivative> Derivative { get; }
        public IPersistenceSqlRepository<User> User { get; }
    }
}
