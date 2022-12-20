using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Exceptions;
using Shared.Persistence.Settings.Connections;

using System.Linq.Expressions;

namespace Shared.Persistence.Contexts;

public abstract class MongoContext : IDisposable
{
    internal IMongoDatabase DataBase { get; }
    private IClientSession? _clientSession;

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

        var entities = await (await collection.FindAsync(condition, null, cToken)).ToListAsync();

        if (!entities.Any())
            return Array.Empty<T>();

        var entityProperties = typeof(T).GetProperties();
        var entityPropertiesDictionary = entityProperties.ToDictionary(x => x.Name, x => x.GetValue(entity));

        for (int i = 0; i < entities.Count; i++)
            for (int j = 0; j < entityProperties.Length; j++)
            {
                var newValue = entityPropertiesDictionary[entityProperties[j].Name];

                if (newValue == default)
                    continue;

                var oldValue = entityProperties[j].GetValue(entities[i]);

                if (oldValue != newValue)
                    entityProperties[j].SetValue(entities[i], newValue);

                var replacedResult = await collection.FindOneAndReplaceAsync(condition, entities[i]);

                if (replacedResult is null)
                    throw new SharedPersistenceException(nameof(MongoContext), nameof(UpdateAsync), new("FindAndReplace method does bed request"));
            }

        return entities.ToArray();
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

    public async Task SetTransactionAsync(CancellationToken cToken)
    {
        _clientSession ??= await DataBase.Client.StartSessionAsync(null, cToken);
        _clientSession.StartTransaction();
    }
    public Task CommitTransactionAsync(CancellationToken cToken) => _clientSession is null
        ? throw new SharedPersistenceException(nameof(MongoContext), nameof(CommitTransactionAsync), new("Mongo client session was not found"))
        : _clientSession.CommitTransactionAsync(cToken);
    public Task RollbackTransactionAsync(CancellationToken cToken) => _clientSession is null
        ? throw new SharedPersistenceException(nameof(MongoContext), nameof(RollbackTransactionAsync), new("Mongo client session was not found"))
        : _clientSession.AbortTransactionAsync(cToken);

    public void Dispose() => _clientSession?.Dispose();
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