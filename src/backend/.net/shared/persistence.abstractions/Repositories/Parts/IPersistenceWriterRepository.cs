using Shared.Models.Results;
using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Abstractions.Entities.Catalogs;

using System.Linq.Expressions;

namespace Shared.Persistence.Abstractions.Repositories.Parts
{
    public interface IPersistenceWriterRepository<T> where T : class, IPersistent
    {
        Task CreateAsync(T entity, CancellationToken? cToken = null);
        Task CreateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? cToken = null);
        Task<TryResult<T>> TryCreateAsync(T entity, CancellationToken? cToken = null);
        Task<TryResult<T[]>> TryCreateRangeAsync(IReadOnlyCollection<T> entities, CancellationToken? cToken = null);

        Task<T[]> UpdateAsync(Expression<Func<T, bool>> condition, T entity, CancellationToken? cToken = null);
        Task<TryResult<T[]>> TryUpdateAsync(Expression<Func<T, bool>> condition, T entity, CancellationToken? cToken = null);

        Task<T[]> DeleteAsync(Expression<Func<T, bool>> condition, CancellationToken? cToken = null);
        Task<TryResult<T[]>> TryDeleteAsync(Expression<Func<T, bool>> condition, CancellationToken? cToken = null);

        Task SaveProcessableAsync<TProcessable>(IProcessStep? step, IEnumerable<TProcessable> entities, CancellationToken cToken) where TProcessable : class, T, IPersistentProcess;
    }
}
