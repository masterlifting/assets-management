using AM.Services.Portfolio.Core.Domain.EntityValueObjects;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;

public interface IAccountRepository : IEntityRepository<Account>
{
    Task<Dictionary<string, int>> GetGroupedAccountsByProviderAsync(ProviderId providerId, CancellationToken cToken);
}