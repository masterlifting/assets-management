using MongoDB.Bson;
using MongoDB.Driver;

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

    private IMongoCollection<T> GetCollection<T>() where T : class, IPersistent => DataBase.GetCollection<T>(typeof(T).Name);
    public IQueryable<T> Set<T>() where T : class, IPersistent => GetCollection<T>().AsQueryable();

    public async Task CreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IPersistent
    {
        var collection = GetCollection<T>();
        collection.Find(x => x.Info == "");
        await collection.InsertOneAsync(entity);
        var count = await collection.CountDocumentsAsync(new BsonDocument());
    }
    public async Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
    {
        var collection = GetCollection<T>();
        collection.Find(x => x.Info == "");
        await collection.InsertManyAsync(entities);
        var count = await collection.CountDocumentsAsync(new BsonDocument());

        throw new NotImplementedException();
    }

    public Task UpdateAsync<T>(object[] id, T entity, CancellationToken? ctToken = null) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }
    public Task UpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }

    public Task<T> DeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }
    public Task DeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
    {
        throw new NotImplementedException();
    }
}
public sealed class MongoDBModelBuilder
{
    private readonly IMongoDatabase _database;
    public MongoDBModelBuilder(IMongoDatabase database) => _database = database;

    public void SetCollection<T>(CreateCollectionOptions options) where T : class, IPersistent
    {
        var collection = _database.GetCollection<T>(typeof(T).Name);
        
        if (collection is null)
            _database.CreateCollection(typeof(T).Name, options);
    }
}
