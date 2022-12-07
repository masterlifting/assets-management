using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Settings.Connections;

namespace Shared.Persistence.Contexts;

public abstract class MongoDBContext
{
    internal IMongoDatabase DataBase { get; }

    protected MongoDBContext(MongoDBConnectionSettings connectionSettings)
    {
        var connectionString = connectionSettings.GetConnectionString();
        var client = new MongoClient(connectionString);
        DataBase = client.GetDatabase(connectionSettings.Database);
        OnModelCreating(new MongoDBModelBuilder(DataBase));
    }

    public virtual void OnModelCreating(MongoDBModelBuilder builder)
    {
    }

    public IMongoCollection<T> GetCollection<T>() where T : class, IPersistent => DataBase.GetCollection<T>(typeof(T).Name);
    public IMongoQueryable<T> SetCollection<T>() where T : class, IPersistent => GetCollection<T>().AsQueryable();

    public Task CreateAsync<T>(T entity) where T : class, IPersistent =>
        GetCollection<T>().InsertOneAsync(entity);
    public Task CreateAsync<T>(T entity, CancellationToken cToken) where T : class, IPersistent =>
        GetCollection<T>().InsertOneAsync(entity, null, cToken);
    public Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities) where T : class, IPersistent =>
        GetCollection<T>().InsertManyAsync(entities);
    public Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken cToken) where T : class, IPersistent =>
        GetCollection<T>().InsertManyAsync(entities, null, cToken);

    public async Task UpdateAsync<T>(object[] id, T entity) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }
    public Task UpdateAsync<T>(object[] id, T entity, CancellationToken ctToken) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }
    public Task UpdateRangeAsync<T>(IReadOnlyCollection<T> entities) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }
    public Task UpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken cToken) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }


    public Task<T> DeleteAsync<T>(object[] id) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }
    public Task<T> DeleteAsync<T>(object[] id, CancellationToken cToken) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }
    public Task DeleteRangeAsync<T>(IReadOnlyCollection<T> entities) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }
    public Task DeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken cToken) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }
}
public sealed class MongoDBModelBuilder
{
    private readonly IMongoDatabase _database;
    public MongoDBModelBuilder(IMongoDatabase database) => _database = database;

    public IMongoCollection<T> SetCollection<T>(IEnumerable<T>? items = null, CreateCollectionOptions? options = null) where T : class, IPersistent
    {
        var collection = _database.GetCollection<T>(typeof(T).Name);

        if (collection is null)
        {
            _database.CreateCollection(typeof(T).Name, options);
            collection = _database.GetCollection<T>(typeof(T).Name);
        }
        if (items is not null && items.Any() && collection.CountDocuments(new BsonDocument()) == 0)
            collection.InsertMany(items);

        return collection;
    }
}