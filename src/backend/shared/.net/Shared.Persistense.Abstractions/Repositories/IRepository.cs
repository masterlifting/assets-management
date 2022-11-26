using Shared.Contracts.Models.Results;
using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace Shared.Persistense.Abstractions.Repositories
{
    public interface IRepository
    {
        Task CreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IEntity;
        Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity;
        Task<Result> TryCreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IEntity;
        Task<Result> TryCreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity;

        Task UpdateAsync<T>(object[] id, T entity, CancellationToken? ctToken = null) where T : class, IEntity;
        Task UpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity;
        Task<Result> TryUpdateAsync<T>(object[] id, T entity, CancellationToken? cToken = null) where T : class, IEntity;
        Task<Result> TryUpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity;

        Task<T> DeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IEntity;
        Task DeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity;
        Task<Result> TryDeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity;
        Task<ResultData<T>> TryDeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IEntity;

        Task<T[]> GetCatalogsAsync<T>() where T : class, IEntityCatalog;
        Task<Dictionary<int, T>> GetCatalogsDictionaryByIdAsync<T>() where T : class, IEntityCatalog;
        Task<Dictionary<string, T>> GetCatalogsDictionaryByNameAsync<T>() where T : class, IEntityCatalog;
        ValueTask<T?> GetCatalogByIdAsync<T>(int id) where T : class, IEntityCatalog;
        Task<T?> GetCatalogByNameAsync<T>(string name) where T : class, IEntityCatalog;

        Task<Guid[]> PrepareProcessableEntityDataAsync<T>(IProcessableEntityStep step, int limit, CancellationToken cToken) where T : class, IProcessableEntity;
        Task<Guid[]> PrepareProcessableEntityRetryDataAsync<T>(IProcessableEntityStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where T : class, IProcessableEntity;
        Task<T[]> GetProcessableEntityDataAsync<T>(IProcessableEntityStep step, IEnumerable<Guid> ids, CancellationToken cToken) where T : class, IProcessableEntity;
        Task SaveProcessableEntityResultAsync<T>(IProcessableEntityStep? step, IEnumerable<T> entities, CancellationToken cToken) where T : class, IProcessableEntity;
    }
}