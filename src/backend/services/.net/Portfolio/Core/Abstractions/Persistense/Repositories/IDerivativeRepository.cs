using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;

public interface IDerivativeRepository : IEntityStateRepository<Derivative>
{
    Task<Dictionary<string, string[]>> GetGroupedDerivativesAsync(CancellationToken cToken);
}