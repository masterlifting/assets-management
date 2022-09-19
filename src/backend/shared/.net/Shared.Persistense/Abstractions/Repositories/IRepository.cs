using Microsoft.EntityFrameworkCore;

using Shared.Persistense.Abstractions.Entities;

namespace Shared.Persistense.Abstractions.Repositories;

public interface IRepository<T> where T : class, IEntity
{
    DbSet<T> DbSet { get; }
    Task CreateAsync(T entity, CancellationToken? cToken = null);
    Task CreateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? ctToken = null);
    Task UpdateAsync(object[] id, T entity, CancellationToken? ctToken = null);
    Task UpdateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? ctToken = null);
    Task<T> DeleteAsync(object[] id, CancellationToken? ctToken = null);
    Task DeleteRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? ctToken = null);
    Task<bool> TryCreateAsync(T entity, CancellationToken? ctToken = null, out string error);
    Task<bool> TryCreateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? ctToken = null, out string error);
    Task<bool> TryUpdateAsync(object[] id, T entity, CancellationToken? ctToken = null, out string error);
    Task<bool> TryUpdateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? ctToken = null, out string error);
    Task<bool> TryDeleteAsync(object[] id, CancellationToken? ctToken = null, out string error);
    Task<bool> TryDeleteRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? ctToken = null, out string error);

}