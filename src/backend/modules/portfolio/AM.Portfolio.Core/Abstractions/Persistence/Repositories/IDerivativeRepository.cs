using AM.Portfolio.Core.Persistence.Entities.Sql;

namespace AM.Portfolio.Core.Abstractions.Persistence.Repositories;

public interface IDerivativeRepository
{
    Task Create(Derivative derivative, CancellationToken cToken);
    Task Create(IEnumerable<Derivative> derivatives, CancellationToken cToken);
    Task<Derivative[]> Get(CancellationToken cToken);
    Task<Derivative[]> Get(int assetId, CancellationToken cToken);
    Task<Derivative[]> Get(IEnumerable<int> assetsId, CancellationToken cToken);
}
