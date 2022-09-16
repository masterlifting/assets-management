using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using Shared.Infrastructure.Persistense.Repositories.Interface;

namespace AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

public interface IDerivativeRepository : IEntityStateRepository<Derivative>
{
    Task<Dictionary<string, string[]>> GetGroupedDerivativesAsync(CancellationToken cToken);
}