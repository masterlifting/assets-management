using Shared.Contracts.Models.Results;
using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace Shared.Persistense.Abstractions.Repositories;

public interface IRepository
{
    Task<T?> FindAsync<T>(params object[] id) where T : class, IPersistensable;
    Task<T?> FindAsync<T, TId>(TId id) where T : class, IPersistensable where TId : struct;

    Task CreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IPersistensable;
    Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistensable;
    Task<Result> TryCreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IPersistensable;
    Task<Result> TryCreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistensable;

    Task UpdateAsync<T>(object[] id, T entity, CancellationToken? ctToken = null) where T : class, IPersistensable;
    Task UpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistensable;
    Task<Result> TryUpdateAsync<T>(object[] id, T entity, CancellationToken? cToken = null) where T : class, IPersistensable;
    Task<Result> TryUpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistensable;

    Task<T> DeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IPersistensable;
    Task DeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistensable;
    Task<Result> TryDeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistensable;
    Task<ResultData<T>> TryDeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IPersistensable;

    Task<T[]> GetCatalogsAsync<T>() where T : class, IPersistensableCatalog;
    Task<Dictionary<int, T>> GetCatalogsDictionaryByIdAsync<T>() where T : class, IPersistensableCatalog;
    Task<Dictionary<string, T>> GetCatalogsDictionaryByNameAsync<T>() where T : class, IPersistensableCatalog;
    Task<T?> GetCatalogByIdAsync<T>(int id) where T : class, IPersistensableCatalog;
    Task<T?> GetCatalogByNameAsync<T>(string name) where T : class, IPersistensableCatalog;

    Task<Guid[]> GetPreparedProcessableIdsAsync<T>(IProcessableStep step, int limit, CancellationToken cToken) where T : class, IPersistensableProcess;
    Task<Guid[]> GetPrepareUnprocessableIdsAsync<T>(IProcessableStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where T : class, IPersistensableProcess;
    Task<T[]> GetProcessableEntitiesAsync<T>(IProcessableStep step, IEnumerable<Guid> ids, CancellationToken cToken) where T : class, IPersistensableProcess;
    Task SaveProcessableEntitiesAsync<T>(IProcessableStep? step, IEnumerable<T> entities, CancellationToken cToken) where T : class, IPersistensableProcess;
}