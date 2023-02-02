using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Contexts;
using Shared.Persistence.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistence
{
    public interface IUnitOfWorkRepository
    {
        IPostgrePersistenceContext PostgreContext { get; }
        IMongoPersistenceContext MongoContext { get; }

        IPersistenceSqlRepository<ProcessStep> ProcessStep { get; }

        IIncomingDataRepository IncomingData { get; }
        IAssetRepository Asset { get; }
        IDealRepository Deal { get; }
        IEventRepository Event { get; }
        IDerivativeRepository Derivative { get; }
        IUserRepository User { get; }
        IAccountRepository Account { get; }
    }
}
