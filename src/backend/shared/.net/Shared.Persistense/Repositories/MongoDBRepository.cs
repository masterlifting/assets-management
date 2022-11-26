using Microsoft.Extensions.Logging;
using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities;
using Shared.Contracts.Models.Results;
using Shared.Persistense.Abstractions.Entities.Catalogs;
using Shared.Persistense.Contexts;
using Shared.Persistense.Abstractions.Repositories;

namespace Shared.Persistense.Repositories;

public class MongoDBRepository : IMongoDBRepository
{
    private readonly string _initiator;
    private readonly ILogger _logger;
    private readonly MongoDBContext _context;

    //MongoClient _client{}
    public MongoDBRepository(ILogger logger, MongoDBContext context)
    {
        _logger = logger;
        _context = context;
        var objectId = base.GetHashCode();
        _initiator = $"{nameof(MongoDBRepository)} ({objectId})";
        //_client = new MongoClient("mongodb://localhost:27017"){}
    }

    public virtual async Task CreateAsync<T>(T entity, CancellationToken? ctToken = null) where T : class, IEntity
    {
        if (!ctToken.HasValue)
            await _context.CreateAsync(entity);
        else
            await _context.CreateAsync(entity, ctToken.Value);

        _logger.LogTrace(_initiator, Constants.Actions.Create, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryCreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IEntity
    {
        try
        {
            await CreateAsync(entity, cToken);
            return new Result(true);
        }
        catch (Exception exception)
        {
            return new Result(false, exception.InnerException?.Message ?? exception.Message);
        }
    }
    public virtual async Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Create, Constants.Actions.NoData);
            return;
        }

        if (!cToken.HasValue)
            await _context.CreateRangeAsync(entities);
        else
            await _context.CreateRangeAsync(entities, cToken.Value);

        _logger.LogTrace(_initiator, Constants.Actions.Create, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryCreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
    {
        try
        {
            await CreateRangeAsync(entities, cToken);
            return new Result(true);
        }
        catch (Exception exception)
        {
            return new Result(false, exception.InnerException?.Message ?? exception.Message);
        }
    }

    public virtual async Task UpdateAsync<T>(object[] id, T entity, CancellationToken? ctToken = null) where T : class, IEntity
    {
        if (!ctToken.HasValue)
            await _context.UpdateAsync(id, entity);
        else
            await _context.UpdateAsync(id, entity, ctToken.Value);

        _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryUpdateAsync<T>(object[] id, T entity, CancellationToken? cToken = null) where T : class, IEntity
    {
        try
        {
            await UpdateAsync(id, entity, cToken);
            return new Result(true);
        }
        catch (Exception exception)
        {
            return new Result(false, exception.InnerException?.Message ?? exception.Message);
        }
    }
    public virtual async Task UpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.NoData);
            return;
        }

        if (!cToken.HasValue)
            await _context.UpdateRangeAsync(entities);
        else
            await _context.UpdateRangeAsync(entities, cToken.Value);

        _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryUpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
    {
        try
        {
            await UpdateRangeAsync(entities, cToken);
            return new Result(true);
        }
        catch (Exception exception)
        {
            return new Result(false, exception.InnerException?.Message ?? exception.Message);
        }
    }

    public virtual async Task<T> DeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IEntity
    {
        T entity;
        if (!cToken.HasValue)
            entity = await _context.DeleteAsync<T>(id);
        else
            entity = await _context.DeleteAsync<T>(id, cToken.Value);

        _logger.LogTrace(_initiator, Constants.Actions.Delete, Constants.Actions.Success);

        return entity;
    }
    public virtual async Task<ResultData<T>> TryDeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IEntity
    {
        try
        {
            var entity = await DeleteAsync<T>(id, cToken);
            return new ResultData<T>(new(true), entity);
        }
        catch (Exception exception)
        {
            return new ResultData<T>(new(false, exception.InnerException?.Message ?? exception.Message), null);
        }
    }
    public virtual async Task DeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.NoData);
            return;
        }

        if (!cToken.HasValue)
            await _context.DeleteRangeAsync(entities);
        else
            await _context.DeleteRangeAsync(entities, cToken.Value);

        _logger.LogTrace(_initiator, Constants.Actions.Delete, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryDeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
    {
        try
        {
            await DeleteRangeAsync(entities, cToken);
            return new Result(true);
        }
        catch (Exception exception)
        {
            return new Result(false, exception.InnerException?.Message ?? exception.Message);
        }
    }

    public Task<Dictionary<int, T>> GetCatalogsDictionaryByIdAsync<T>() where T : class, IEntityCatalog => _context.GetCatalogsDictionaryByIdAsync<T>();
    public Task<Dictionary<string, T>> GetCatalogsDictionaryByNameAsync<T>() where T : class, IEntityCatalog => _context.GetCatalogsDictionaryByNameAsync<T>();
    public ValueTask<T?> GetCatalogByIdAsync<T>(int id) where T : class, IEntityCatalog => _context.GetCatalogByIdAsync<T>(id);
    public Task<T?> GetCatalogByNameAsync<T>(string name) where T : class, IEntityCatalog => _context.GetCatalogByNameAsync<T>(name);
    public Task<T[]> GetCatalogsAsync<T>() where T : class, IEntityCatalog => _context.GetCatalogsAsync<T>();

    public Task<Guid[]> PrepareProcessableEntityDataAsync<T>(IProcessableEntityStep step, int limit, CancellationToken cToken) where T : class, IProcessableEntity
        => _context.PrepareProcessableEntityDataAsync<T>(step, limit, cToken);
    public Task<Guid[]> PrepareProcessableEntityRetryDataAsync<T>(IProcessableEntityStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where T : class, IProcessableEntity
        => _context.PrepareProcessableEntityRetryDataAsync<T>(step, limit, updateTime, maxAttempts, cToken);

    public Task<T[]> GetProcessableEntityDataAsync<T>(IProcessableEntityStep step, IEnumerable<Guid> ids, CancellationToken cToken) where T : class, IProcessableEntity 
        => _context.GetProcessableEntityDataAsync<T>(step, ids, cToken);

    public Task SaveProcessableEntityResultAsync<T>(IProcessableEntityStep? step, IEnumerable<T> entities, CancellationToken cToken) where T : class, IProcessableEntity 
        => _context.SaveProcessableEntityResultAsync(step, entities, cToken);

    public void Test()
    {
        //var db = _client.GetDatabase("test_db"){}
        //var collection = db.GetCollection<TestMongoDbModel>("test_collection"){}
        //collection.InsertOne(new TestMongoDbModel()){}
        //collection.InsertMany(Enumerable.Range(0, 5).Select(x => new TestMongoDbModel())){}
    }
}