using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using Shared.Infrastructure.Persistense.Repositories.Interface;

namespace AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<Dictionary<string, int>> GetGroupedAccountsByProviderAsync(ProviderId providerId, CancellationToken cToken);
}