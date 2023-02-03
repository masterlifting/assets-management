using Shared.Persistence.Abstractions.Entities;

namespace Shared.Persistence.Abstractions.Contexts
{
    public interface IPostgrePersistenceContext : IPersistenceContext<IPersistentSql>
    {
        Task<T?> FindByIdAsync<T>(CancellationToken cToken = default, object[] id) where T : class, IPersistentSql;
    }
}
