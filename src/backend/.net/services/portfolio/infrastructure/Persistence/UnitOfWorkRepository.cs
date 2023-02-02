using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Contexts;
using Shared.Persistence.Abstractions.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        public UnitOfWorkRepository(
            IPostgrePersistenceContext postgreContext
            , IMongoPersistenceContext mongoContext
            , IPersistenceSqlRepository<ProcessStep> processStep
            , IIncomingDataRepository incomingData
            , IAssetRepository asset
            , IDealRepository deal
            , IEventRepository _event
            , IDerivativeRepository derivative
            , IUserRepository user
            , IAccountRepository account)
        {
            PostgreContext = postgreContext;
            MongoContext = mongoContext;
            ProcessStep = processStep;
            IncomingData = incomingData;
            Asset = asset;
            Deal = deal;
            Event = _event;
            Derivative = derivative;
            User = user;
            Account = account;
        }

        public IPostgrePersistenceContext PostgreContext { get; }
        public IMongoPersistenceContext MongoContext { get; }
        public IPersistenceSqlRepository<ProcessStep> ProcessStep { get; }
        public IIncomingDataRepository IncomingData { get; }
        public IAssetRepository Asset { get; }
        public IDealRepository Deal { get; }
        public IEventRepository Event { get; }
        public IDerivativeRepository Derivative { get; }
        public IUserRepository User { get; }
        public IAccountRepository Account { get; }
    }
}
