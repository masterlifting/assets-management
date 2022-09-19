using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using Shared.Infrastructure.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

public interface IDerivativeRepository : IEntityStateRepository<Derivative>
{
    Task<Dictionary<string, string[]>> GetGroupedDerivativesAsync(CancellationToken cToken);
}