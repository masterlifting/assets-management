using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Abstractions.Entities.Catalogs;

using System.Linq.Expressions;

namespace Shared.Persistence.Abstractions.Repositories.Parts
{
    public interface IPersistenceReaderRepository<T> where T : class, IPersistent
    {
        Task<T?> FindSingleAsync(Expression<Func<T, bool>> condition);
        Task<T?> FindFirstAsync(Expression<Func<T, bool>> condition);
        Task<T[]> FindManyAsync(Expression<Func<T, bool>> condition);

        Task<TCatalog[]> GetCatalogsAsync<TCatalog>() where TCatalog : class, T, IPersistentCatalog;
        Task<Dictionary<int, TCatalog>> GetCatalogsDictionaryByIdAsync<TCatalog>() where TCatalog : class, T, IPersistentCatalog;
        Task<Dictionary<string, TCatalog>> GetCatalogsDictionaryByNameAsync<TCatalog>() where TCatalog : class, T, IPersistentCatalog;
        Task<TCatalog?> GetCatalogByIdAsync<TCatalog>(int id) where TCatalog : class, T, IPersistentCatalog;
        Task<TCatalog?> GetCatalogByNameAsync<TCatalog>(string name) where TCatalog : class, T, IPersistentCatalog;

        Task<TProcessable[]> GetProcessableAsync<TProcessable>(IProcessStep step, int limit, CancellationToken cToken) where TProcessable : class, T, IPersistentProcess;
        Task<TProcessable[]> GetUnprocessableAsync<TProcessable>(IProcessStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where TProcessable : class, T, IPersistentProcess;
    }
}
