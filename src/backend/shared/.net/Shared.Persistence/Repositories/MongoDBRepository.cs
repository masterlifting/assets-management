using Microsoft.Extensions.Logging;

using MongoDB.Driver;

using Shared.Contracts.Models.Results;
using Shared.Extensions.Logging;
using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Abstractions.Entities.Catalogs;
using Shared.Persistence.Abstractions.Repositories;
using Shared.Persistence.Contexts;

using static Shared.Persistence.Abstractions.Constants.Enums;

namespace Shared.Persistence.Repositories;

public class MongoDBRepository<TContext> : IMongoDBRepository where TContext : MongoDBContext
{
    private readonly string _initiator;
    private readonly ILogger _logger;
    private readonly TContext _context;

    public MongoDBRepository(ILogger<MongoDBRepository<TContext>> logger, TContext context)
    {
        _logger = logger;
        _context = context;
        var objectId = base.GetHashCode();
        _initiator = $"{nameof(MongoDBRepository<TContext>)} ({objectId})";
    }

    public virtual async Task CreateAsync<T>(T entity, CancellationToken? ctToken = null) where T : class, IPersistent
    {
        if (!ctToken.HasValue)
            await _context.CreateAsync(entity);
        else
            await _context.CreateAsync(entity, ctToken.Value);

        _logger.LogTrace(_initiator, typeof(T).Name + ' ' + Constants.Actions.Created, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryCreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IPersistent
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
    public virtual async Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Created, Constants.Actions.NoData);
            return;
        }

        if (!cToken.HasValue)
            await _context.CreateRangeAsync(entities);
        else
            await _context.CreateRangeAsync(entities, cToken.Value);

        _logger.LogTrace(_initiator, Constants.Actions.Created, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryCreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
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

    public virtual async Task UpdateAsync<T>(object[] id, T entity, CancellationToken? ctToken = null) where T : class, IPersistent
    {
        if (!ctToken.HasValue)
            await _context.UpdateAsync(id, entity);
        else
            await _context.UpdateAsync(id, entity, ctToken.Value);

        _logger.LogTrace(_initiator, Constants.Actions.Updated, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryUpdateAsync<T>(object[] id, T entity, CancellationToken? cToken = null) where T : class, IPersistent
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
    public virtual async Task UpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Updated, Constants.Actions.NoData);
            return;
        }

        if (!cToken.HasValue)
            await _context.UpdateRangeAsync(entities);
        else
            await _context.UpdateRangeAsync(entities, cToken.Value);

        _logger.LogTrace(_initiator, Constants.Actions.Updated, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryUpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
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

    public virtual async Task<T> DeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IPersistent
    {
        T entity;
        if (!cToken.HasValue)
            entity = await _context.DeleteAsync<T>(id);
        else
            entity = await _context.DeleteAsync<T>(id, cToken.Value);

        _logger.LogTrace(_initiator, Constants.Actions.Deleted, Constants.Actions.Success);

        return entity;
    }
    public virtual async Task<ResultData<T>> TryDeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IPersistent
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
    public virtual async Task DeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Updated, Constants.Actions.NoData);
            return;
        }

        if (!cToken.HasValue)
            await _context.DeleteRangeAsync(entities);
        else
            await _context.DeleteRangeAsync(entities, cToken.Value);

        _logger.LogTrace(_initiator, Constants.Actions.Deleted, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryDeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
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

    public Task<T[]> GetCatalogsAsync<T>() where T : class, IPersistentCatalog => Task.Run(() => 
        _context.SetCollection<T>().ToArray());
    public Task<Dictionary<int, T>> GetCatalogsDictionaryByIdAsync<T>() where T : class, IPersistentCatalog => 
            Task.Run(() => _context.SetCollection<T>().ToDictionary(x => x.Id));
    public Task<Dictionary<string, T>> GetCatalogsDictionaryByNameAsync<T>() where T : class, IPersistentCatalog => 
            Task.Run(() => _context.SetCollection<T>().ToDictionary(x => x.Name));
    public Task<T?> GetCatalogByIdAsync<T>(int id) where T : class, IPersistentCatalog => 
        Task.Run(() => _context.SetCollection<T>().FirstOrDefault(x => x.Id == id));
    public Task<T?> GetCatalogByNameAsync<T>(string name) where T : class, IPersistentCatalog => 
        Task.Run(() => _context.SetCollection<T>().FirstOrDefault(x => x.Name.Equals(name)));

    public async Task<T[]> GetProcessableAsync<T>(IProcessStep step, int limit, CancellationToken cToken) where T : class, IPersistentProcess
    {
        var collection = _context.GetCollection<T>();

        var builder = Builders<T>.Update
            .Set(x => x.Updated, DateTime.UtcNow)
            .Set(x => x.ProcessStatusId, (int)ProcessStatuses.Processing)
            .Inc(x => x.ProcessAttempt, 1);
        
        List<T> result = new(limit);
        
        foreach (var item in Enumerable.Range(0, limit))
        {
            var entity = await collection.FindOneAndUpdateAsync(x => 
            x.ProcessStepId == step.Id 
            && x.ProcessStatusId == (int)ProcessStatuses.Ready
            , builder);
            
            if (entity is null)
                break;
            
            result.Add(entity);
        }
        
        return result.ToArray();
    }
    public async Task<T[]> GetUnprocessableAsync<T>(IProcessStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where T : class, IPersistentProcess
    {
        var collection = _context.GetCollection<T>();

        var builder = Builders<T>.Update
            .Set(x => x.Updated, DateTime.UtcNow)
            .Set(x => x.ProcessStatusId, (int)ProcessStatuses.Processing)
            .Inc(x => x.ProcessAttempt, 1);

        List<T> result = new(limit);

        foreach (var item in Enumerable.Range(0, limit))
        {
            var entity = await collection.FindOneAndUpdateAsync(x => 
            x.ProcessStepId == step.Id 
            && ((x.ProcessStatusId == (int)ProcessStatuses.Processing && x.Updated < updateTime) || (x.ProcessStatusId == (int)ProcessStatuses.Error))
            && (x.ProcessAttempt < maxAttempts)
            , builder);

            if (entity is null)
                break;

            result.Add(entity);
        }

        return result.ToArray();
    }
    public Task SaveProcessableAsync<T>(IProcessStep? step, IEnumerable<T> entities, CancellationToken cToken) where T : class, IPersistentProcess
    {
        var array = entities.ToArray();

        if (step is null)
            return UpdateRangeAsync(array, cToken);

        foreach (var entity in array.Where(x => x.ProcessStatusId != (int)ProcessStatuses.Error))
            entity.ProcessStepId = step.Id;

        return UpdateRangeAsync(array, cToken);
    }

    public Task<T?> FindAsync<T>(params object[] id) where T : class, IPersistent
    {
        throw new NotImplementedException(nameof(FindAsync));
    }
    public Task<T?> FindAsync<T, TId>(TId id) where T : class, IPersistent where TId : struct
    {
        throw new NotImplementedException(nameof(FindAsync));
    }
}