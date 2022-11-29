﻿using MongoDB.Bson;
using MongoDB.Driver;

using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Settings.Connections;

namespace Shared.Persistence.Contexts;

public abstract class MongoDBContext
{
    internal IMongoDatabase DataBase { get; }
    protected MongoDBContext(MongoDBConnectionSettings connectionSettings)
    {
        DataBase = new MongoClient(connectionSettings.GetConnectionString()).GetDatabase(connectionSettings.Database);
    }

    //var collection = db.GetCollection<TestMongoDbModel>("test_collection"){}
    //collection.InsertOne(new TestMongoDbModel()){}
    //collection.InsertMany(Enumerable.Range(0, 5).Select(x => new TestMongoDbModel())){}

    private IMongoCollection<T> GetCollection<T>() where T : class, IPersistent => DataBase.GetCollection<T>(typeof(T).Name);
    public IQueryable<T> Set<T>() where T : class, IPersistent => GetCollection<T>().AsQueryable();

    public Task CreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IPersistent
    {
        throw new NotImplementedException();
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