using System.Linq.Expressions;

using Shared.Persistence.Abstractions.Entities;

namespace Shared.Persistence.Abstractions.Contexts
{
    public interface IPostgrePersistenceContext : IPersistenceContext<IPersistentSql>
    {
        string GetTableName<T>() where T : class, IPersistentSql;
        Task<T?> FindByIdAsync<T>(CancellationToken cToken = default, params object[] id) where T : class, IPersistentSql;
        Task<T[]> UpdateAsync<T>(Expression<Func<T, bool>> condition, T entity, CancellationToken cToken = default) where T : class, IPersistentSql;
        Task<T[]> ExecuteQueryAsync<T>(string query, CancellationToken cToken = default) where T : class, IPersistentSql;
    }
}
