using Shared.Models.Results;
using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Abstractions.Entities.Catalogs;

namespace Shared.Persistence.Abstractions.Repositories;

public interface IPersistenceRepository
{
    Task<T?> FindAsync<T>(params object[] id) where T : class, IPersistent;
    Task<T?> FindAsync<T, TId>(TId id) where T : class, IPersistent where TId : struct;

    Task CreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IPersistent;
    Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent;
    Task<Result> TryCreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IPersistent;
    Task<Result> TryCreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent;

    Task UpdateAsync<T>(object[] id, T entity, CancellationToken? ctToken = null) where T : class, IPersistent;
    Task UpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent;
    Task<Result> TryUpdateAsync<T>(object[] id, T entity, CancellationToken? cToken = null) where T : class, IPersistent;
    Task<Result> TryUpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent;

    Task<T> DeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IPersistent;
    Task DeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent;
    Task<Result> TryDeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent;
    Task<ResultData<T>> TryDeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IPersistent;

    Task<T[]> GetCatalogsAsync<T>() where T : class, IPersistentCatalog;
    Task<Dictionary<int, T>> GetCatalogsDictionaryByIdAsync<T>() where T : class, IPersistentCatalog;
    Task<Dictionary<string, T>> GetCatalogsDictionaryByNameAsync<T>() where T : class, IPersistentCatalog;
    Task<T?> GetCatalogByIdAsync<T>(int id) where T : class, IPersistentCatalog;
    Task<T?> GetCatalogByNameAsync<T>(string name) where T : class, IPersistentCatalog;

    Task<T[]> GetProcessableAsync<T>(IProcessStep step, int limit, CancellationToken cToken) where T : class, IPersistentProcess;
    Task<T[]> GetUnprocessableAsync<T>(IProcessStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where T : class, IPersistentProcess;
    Task SaveProcessableAsync<T>(IProcessStep? step, IEnumerable<T> entities, CancellationToken cToken) where T : class, IPersistentProcess;
}