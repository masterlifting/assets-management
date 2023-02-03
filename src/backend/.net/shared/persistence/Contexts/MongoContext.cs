using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Shared.Persistence.Abstractions.Contexts;
using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Exceptions;
using Shared.Persistence.Settings.Connections;

using System.Linq.Expressions;

namespace Shared.Persistence.Contexts;

public abstract class MongoContext : IMongoPersistenceContext
{
    private readonly IMongoDatabase _dataBase;
    private readonly MongoClient _client;
    private IClientSessionHandle? _session;
    private IMongoCollection<T> GetCollection<T>() where T : class, IPersistentNoSql => _dataBase.GetCollection<T>(typeof(T).Name);
    private IMongoQueryable<T> SetCollection<T>() where T : class, IPersistentNoSql => GetCollection<T>().AsQueryable();

    protected MongoContext(MongoDBConnectionSettings connectionSettings)
    {
        var connectionString = connectionSettings.GetConnectionString();
        _client = new MongoClient(connectionString);
        _dataBase = _client.GetDatabase(connectionSettings.Database);
        OnModelCreating(new MongoModelBuilder(_dataBase));
    }

    public virtual void OnModelCreating(MongoModelBuilder builder)
    {
    }

    public IQueryable<T> Set<T>() where T : class, IPersistentNoSql => SetCollection<T>();

    public Task<T[]> FindManyAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cToken = default) where T : class, IPersistentNoSql =>
            Task.Run(() => Set<T>().Where(condition).ToArray(), cToken);
    public Task<T?> FindFirstAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cToken = default) where T : class, IPersistentNoSql =>
        Task.Run(() => Set<T>().FirstOrDefault(), cToken);
    public Task<T?> FindSingleAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cToken = default) where T : class, IPersistentNoSql =>
        Task.Run(() => Set<T>().SingleOrDefault(), cToken);

    public Task CreateAsync<T>(T entity, CancellationToken cToken = default) where T : class, IPersistentNoSql =>
        GetCollection<T>().InsertOneAsync(entity, null, cToken);
    public Task CreateManyAsync<T>(IReadOnlyCollection<T> entities, CancellationToken cToken) where T : class, IPersistentNoSql =>
        GetCollection<T>().InsertManyAsync(entities, null, cToken);
    public async Task<T[]> UpdateAsync<T>(Expression<Func<T, bool>> condition, T entity, CancellationToken cToken = default) where T : class, IPersistentNoSql
    {
        var entities = await FindManyAsync(condition, cToken);

        if (!entities.Any())
            return Array.Empty<T>();

        var entityProperties = typeof(T).GetProperties();
        var entityPropertiesDictionary = entityProperties.ToDictionary(x => x.Name, x => x.GetValue(entity));

        for (int i = 0; i < entities.Length; i++)
        {
            for (int j = 0; j < entityProperties.Length; j++)
            {
                var newValue = entityPropertiesDictionary[entityProperties[j].Name];

                if (newValue == default)
                    continue;

                var oldValue = entityProperties[j].GetValue(entities[i]);

                if (oldValue != newValue)
                    entityProperties[j].SetValue(entities[i], newValue);
            }

            var replacedResult = GetCollection<T>().FindOneAndReplaceAsync(condition, entities[i], new FindOneAndReplaceOptions<T>
            {
                IsUpsert = false,
                ReturnDocument = ReturnDocument.After
            }, cToken);

            if (replacedResult is null)
                throw new SharedPersistenceException(nameof(MongoContext), nameof(UpdateAsync), new("FindOneAndReplace method does bed request"));
        }

        GetCollection<T>().UpdateMany<T>(condition, null, entity,);

        return entities.ToArray();
    }
    public async Task<T[]> DeleteAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cToken = default) where T : class, IPersistentNoSql
    {
        var collection = GetCollection<T>();

        var count = await collection.CountDocumentsAsync(condition, null, cToken);

        var result = new List<T>((int)count);

        for (int i = 0; i < count; i++)
            result.Add(await collection.FindOneAndDeleteAsync(condition, null, cToken));

        return result.ToArray();
    }

    public async Task SetTransactionAsync(CancellationToken cToken = default)
    {
        _session = await _client.StartSessionAsync();
        _session.StartTransaction();
    }
    public async Task CommitTransactionAsync(CancellationToken cToken = default)
    {
        if(_session is null)
            throw new SharedPersistenceException(nameof(MongoContext), nameof(CommitTransactionAsync), new("The transaction session was not found"));

        await _session.CommitTransactionAsync();
        _session.Dispose();
    }
    public async Task RollbackTransactionAsync(CancellationToken cToken = default)
    {
        if (_session is null)
            throw new SharedPersistenceException(nameof(MongoContext), nameof(RollbackTransactionAsync), new("The transaction session was not found"));

        await _session.CommitTransactionAsync();
        _session.Dispose();
    }

    public void Dispose() => _session?.Dispose();
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
