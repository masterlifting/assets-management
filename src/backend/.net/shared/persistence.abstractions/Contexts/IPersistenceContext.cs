using System.Linq.Expressions;
using Shared.Persistence.Abstractions.Entities;

namespace Shared.Persistence.Abstractions.Contexts
{
    public interface IPersistenceContext<T> : IDisposable where T : class, IPersistent
    {
        IQueryable<T> Set();

        Task<T[]> FindManyAsync(Expression<Func<T, bool>> condition, CancellationToken cToken = default);
        Task<T?> FindFirstAsync(Expression<Func<T, bool>> condition, CancellationToken cToken = default);
        Task<T?> FindSingleAsync(Expression<Func<T, bool>> condition, CancellationToken cToken = default);

        Task CreateAsync(T entity, CancellationToken cToken = default);
        Task<T[]> UpdateAsync(Expression<Func<T, bool>> condition, T entity, CancellationToken cToken = default);
        Task<T[]> DeleteAsync(Expression<Func<T, bool>> condition, CancellationToken cToken = default);

        Task SetTransactionAsync(CancellationToken cToken = default);
        Task CommitTransactionAsync(CancellationToken cToken = default);
        Task RollbackTransactionAsync(CancellationToken cToken = default);
    }
}
