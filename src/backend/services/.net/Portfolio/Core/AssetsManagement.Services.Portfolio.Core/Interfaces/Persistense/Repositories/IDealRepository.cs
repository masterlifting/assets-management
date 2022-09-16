using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using Shared.Infrastructure.Persistense.Repositories.Interface;

namespace AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

public interface IDealRepository : IEntityStateRepository<Deal>
{
    Task<Deal[]> GetFullDealsAsync(IEnumerable<Derivative> derivatives);
}