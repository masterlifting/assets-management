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
    private IMongoDatabase DataBase { get; }

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

    private IMongoCollection<T> Get<T>() where T : class, IPersistentNoSql => DataBase.GetCollection<T>(typeof(T).Name);
    private IMongoQueryable<T> Set<T>() where T : class, IPersistentNoSql => Get<T>().AsQueryable();

    public IQueryable<IPersistentNoSql> Set() => Set<IPersistentNoSql>();

    public Task<IPersistentNoSql?> FindFirstAsync(Expression<Func<IPersistentNoSql, bool>> condition, CancellationToken cToken = default) =>
        Task.Run(() => Set().FirstOrDefault(), cToken);
    public Task<IPersistentNoSql?> FindSingleAsync(Expression<Func<IPersistentNoSql, bool>> condition, CancellationToken cToken = default) =>
        Task.Run(() => Set().SingleOrDefault(), cToken);
    public Task<IPersistentNoSql[]> FindManyAsync(Expression<Func<IPersistentNoSql, bool>> condition, CancellationToken cToken = default) =>
        Task.Run(() => Set().Where(condition).ToArray(), cToken);

    public Task CreateAsync(IPersistentNoSql entity, CancellationToken cToken = default) =>
        Get<IPersistentNoSql>().InsertOneAsync(entity, null, cToken);
    public async Task<IPersistentNoSql[]> UpdateAsync(Expression<Func<IPersistentNoSql, bool>> condition, IPersistentNoSql entity, CancellationToken cToken = default)
    {
        var collection = Get<T>();

        var entities = await (await collection.FindAsync(condition, null, cToken)).ToListAsync(cToken);

        if (!entities.Any())
            return Array.Empty<T>();

        var entityProperties = typeof(T).GetProperties();
        var entityPropertiesDictionary = entityProperties.ToDictionary(x => x.Name, x => x.GetValue(entity));

        for (int i = 0; i < entities.Count; i++)
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

            var replacedResult = await collection.FindOneAndReplaceAsync(condition, entities[i], new FindOneAndReplaceOptions<T>
            {
                IsUpsert = false,
                ReturnDocument = ReturnDocument.After
            }, cToken);

            if (replacedResult is null)
                throw new SharedPersistenceException(nameof(MongoContext), nameof(IPersistenceContext.UpdateAsync), new("FindOneAndReplace method does bed request"));
        }

        return entities.ToArray();
    }
    public Task<IPersistentNoSql[]> DeleteAsync(Expression<Func<IPersistentNoSql, bool>> condition, CancellationToken cToken = default)
    {
        var collection = Get<T>();

        var count = await collection.CountDocumentsAsync(condition, null, cToken);

        var result = new List<T>((int)count);

        for (int i = 0; i < count; i++)
            result.Add(await collection.FindOneAndDeleteAsync(condition, null, cToken));

        return result.ToArray();
    }

    public Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken cToken) where T : class, IPersistentNoSql =>
        Get<T>().InsertManyAsync(entities, null, cToken);

    public Task SetTransactionAsync(CancellationToken cToken = default)
    {
        throw new NotImplementedException();
    }
    public Task CommitTransactionAsync(CancellationToken cToken = default)
    {
        throw new NotImplementedException();
    }
    public Task RollbackTransactionAsync(CancellationToken cToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
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
