using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;

using Shared.Persistence.Abstractions.Contexts;

namespace AM.Services.Portfolio.Core.Abstractions.Persistence
{
    public interface IUnitOfWorkRepository
    {
        IPostgrePersistenceContext PostgreContext { get; }
        IMongoPersistenceContext MongoContext { get; }

        IProcessStepRepository ProcessStep { get; }
        IIncomingDataRepository IncomingData { get; }
        IAssetRepository Asset { get; }
        IDealRepository Deal { get; }
        IEventRepository Event { get; }
        IDerivativeRepository Derivative { get; }
        IUserRepository User { get; }
        IAccountRepository Account { get; }
    }
}
