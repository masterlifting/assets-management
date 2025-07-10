using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;

using Shared.Persistence.Abstractions.Contexts;

namespace AM.Services.Portfolio.Infrastructure.Persistence
{
    public sealed class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        public UnitOfWorkRepository(
            IPostgrePersistenceContext postgreContext
            , IMongoPersistenceContext mongoContext
            , IProcessStepRepository processStep
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

        public IProcessStepRepository ProcessStep { get; }
        public IIncomingDataRepository IncomingData { get; }
        public IAssetRepository Asset { get; }
        public IDealRepository Deal { get; }
        public IEventRepository Event { get; }
        public IDerivativeRepository Derivative { get; }
        public IUserRepository User { get; }
        public IAccountRepository Account { get; }
    }
}
