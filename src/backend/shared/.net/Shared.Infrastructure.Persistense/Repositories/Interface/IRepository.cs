using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Persistense.Entities;

namespace Shared.Infrastructure.Persistense.Repositories.Interface;

public interface IRepository<T> where T : class, IEntity
{
    DbSet<T> DbSet { get; }
    Task CreateAsync(T entity, CancellationToken? ctToken = null);
    Task CreateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? ctToken = null);
    Task UpdateAsync(object[] id, T entity, CancellationToken? ctToken = null);
    Task UpdateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? ctToken = null);
    Task<T> DeleteAsync(object[] id, CancellationToken? ctToken = null);
    Task DeleteRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? ctToken = null);
}