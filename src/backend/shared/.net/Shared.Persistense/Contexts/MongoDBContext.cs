using MongoDB.Driver;

using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace Shared.Persistense.Contexts
{
    public class MongoDBContext 
    {
        private readonly MongoClient _client;

        public MongoDBContext(MongoClient client)
        {
            _client = client;
        }
        public Task<T[]> GetCatalogsAsync<T>() where T : class, IEntityCatalog
        {
            throw new NotImplementedException();
        }
        public Task<Dictionary<int, T>> GetCatalogsDictionaryByIdAsync<T>() where T : class, IEntityCatalog
        {
            throw new NotImplementedException();
        }
        public Task<Dictionary<string, T>> GetCatalogsDictionaryByNameAsync<T>() where T : class, IEntityCatalog
        {
            throw new NotImplementedException();
        }
        public ValueTask<T?> GetCatalogByIdAsync<T>(int id) where T : class, IEntityCatalog
        {
            throw new NotImplementedException();
        }
        public Task<T?> GetCatalogByNameAsync<T>(string name) where T : class, IEntityCatalog
        {
            throw new NotImplementedException();
        }
        
        public Task CreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IEntity
        {
            throw new NotImplementedException();
        }
        public Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
        {
            throw new NotImplementedException();
        }
        
        public Task UpdateAsync<T>(object[] id, T entity, CancellationToken? ctToken = null) where T : class, IEntity
        {
            throw new NotImplementedException();
        }
        public Task UpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
        {
            throw new NotImplementedException();
        }
        
        public Task<T> DeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IEntity
        {
            throw new NotImplementedException();
        }
        public Task DeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
        {
            throw new NotImplementedException();
        }
        
        public Task<Guid[]> PrepareProcessableEntityDataAsync<T>(IProcessableEntityStep step, int limit, CancellationToken cToken) where T : class, IProcessableEntity
        {
            throw new NotImplementedException();
        }
        public Task<Guid[]> PrepareProcessableEntityRetryDataAsync<T>(IProcessableEntityStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where T : class, IProcessableEntity
        {
            throw new NotImplementedException();
        }
        public Task<T[]> GetProcessableEntityDataAsync<T>(IProcessableEntityStep step, IEnumerable<Guid> ids, CancellationToken cToken) where T : class, IProcessableEntity
        {
            throw new NotImplementedException();
        }
        public Task SaveProcessableEntityResultAsync<T>(IProcessableEntityStep? step, IEnumerable<T> entities, CancellationToken cToken) where T : class, IProcessableEntity
        {
            throw new NotImplementedException();
        }
    }
}
