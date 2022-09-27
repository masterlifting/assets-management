using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Dictionary<string, int>> GetGroupedAccountsByProviderAsync(ProviderId providerId, CancellationToken cToken);
    }
}