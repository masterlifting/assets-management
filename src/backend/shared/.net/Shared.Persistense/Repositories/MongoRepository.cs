using MongoDB.Bson;
using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Persistense.Repositories;

public class MongoRepository
{
    MongoClient _client;
    public MongoRepository()
    {
        _client = new MongoClient("mongodb://localhost:27017");
    }

    public void Test()
    {
        var db = _client.GetDatabase("test_db");
        var collection = db.GetCollection<TestMongoDbModel>("test_collection");
        collection.InsertOne(new TestMongoDbModel());
        collection.InsertMany(Enumerable.Range(0, 5).Select(x => new TestMongoDbModel()));
    }
}
public class TestMongoDbModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int[] Data { get; set; }
}
