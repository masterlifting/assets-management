using Shared.Contracts.Models.Results;
using Shared.Persistense.Abstractions.Entities;

namespace Shared.Persistense.Abstractions.Repositories;

public interface IRepository<T> where T : class, IEntity
{
    Task CreateAsync(T entity, CancellationToken? cToken = null);
    Task<Result> TryCreateAsync(T entity, CancellationToken? cToken = null);
    Task CreateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? cToken = null);
    Task<Result> TryCreateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? cToken = null);

    Task UpdateAsync(object[] id, T entity, CancellationToken? ctToken = null);
    Task<Result> TryUpdateAsync(object[] id, T entity, CancellationToken? cToken = null);
    Task UpdateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? cToken = null);
    Task<Result> TryUpdateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? cToken = null);

    Task<T> DeleteAsync(object[] id, CancellationToken? cToken = null);
    Task<ResultData<T>> TryDeleteAsync(object[] id, CancellationToken? cToken = null);
    Task DeleteRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? cToken = null);
    Task<Result> TryDeleteRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? cToken = null);
}