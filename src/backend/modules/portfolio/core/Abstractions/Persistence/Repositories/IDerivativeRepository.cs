using AM.Services.Portfolio.Core.Domain.Persistence.Entities;

using Shared.Persistence.Abstractions.Repositories;

using static AM.Services.Common.Constants.Enums;

namespace AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories
{
    public interface IDerivativeRepository : IPersistenceSqlRepository<Derivative>
    {
        Task<Derivative[]> GetDerivativesAsync(AssetTypes type);
        Task<Derivative[]> GetDerivativesAsync();
    }
}
