using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Exceptions;
using Shared.Persistence.Settings.Connections;

using SharpCompress.Common;

using System.Collections.Immutable;
using System.Linq.Expressions;

namespace Shared.Persistence.Contexts;

public abstract class MongoContext
{
    internal IMongoDatabase DataBase { get; }

    protected MongoContext(MongoDBConnectionSettings connectionSettings)
    {
        var connectionString = connectionSettings.GetConnectionString();
        var client = new MongoClient(connectionString);
        DataBase = client.GetDatabase(connectionSettings.Database);
        OnModelCreating(new MongoModelBuilder(DataBase));
    }

    public virtual void OnModelCreating(MongoModelBuilder builder)
    {
    }

    public IMongoCollection<T> Get<T>() where T : class, IPersistentNoSql => DataBase.GetCollection<T>(typeof(T).Name);
    public IMongoQueryable<T> Set<T>() where T : class, IPersistentNoSql => Get<T>().AsQueryable();

    public Task CreateAsync<T>(T entity, CancellationToken cToken) where T : class, IPersistentNoSql =>
        Get<T>().InsertOneAsync(entity, null, cToken);
    public Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken cToken) where T : class, IPersistentNoSql =>
        Get<T>().InsertManyAsync(entities, null, cToken);
    public async Task<T[]> UpdateAsync<T>(Expression<Func<T, bool>> condition, T entity, CancellationToken cToken) where T : class, IPersistentNoSql
    {
        var collection = Get<T>();
        
        var count = await collection.CountDocumentsAsync(condition, null, cToken);
        
        var result = new List<T>((int)count);
      
        for (int i = 0; i < count; i++)
            result.Add(await collection.FindOneAndReplaceAsync(condition, entity));

        return result.ToArray();
    }
    public async Task<T[]> DeleteAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cToken) where T : class, IPersistentNoSql
    {
        var collection = Get<T>();

        var count = await collection.CountDocumentsAsync(condition, null, cToken);

        var result = new List<T>((int)count);

        for (int i = 0; i < count; i++)
            result.Add(await collection.FindOneAndDeleteAsync(condition));

        return result.ToArray();
    }

    public Task SetTransactionAsync(CancellationToken cToken) => throw new SharedPersistenceException(nameof(MongoContext), nameof(SetTransactionAsync), new(new NotImplementedException()));
    public Task CommitTransactionAsync(CancellationToken cToken) => throw new SharedPersistenceException(nameof(MongoContext), nameof(CommitTransactionAsync), new(new NotImplementedException()));
}
public sealed class MongoModelBuilder
{
    private readonly IMongoDatabase _database;
    public MongoModelBuilder(IMongoDatabase database) => _database = database;

    public IMongoCollection<T> SetCollection<T>(IEnumerable<T>? items = null, CreateCollectionOptions? options = null) where T : class, IPersistentNoSql
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