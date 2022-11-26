using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Contracts.Models.Results;
using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;
using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Contexts;

namespace Shared.Persistense.Repositories;

public class MongoDBRepository : IMongoDBRepository
{
    private readonly string _initiator;
    private readonly ILogger _logger;
    private readonly MongoDBContext _context;

    public MongoDBRepository(ILogger logger, MongoDBContext context)
    {
        _logger = logger;
        _context = context;
        var objectId = base.GetHashCode();
        _initiator = $"{nameof(MongoDBRepository)} ({objectId})";
    }

    public IQueryable<T> Set<T>() where T : class, IEntity => _context.Set<T>();

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

    public Task<T[]> GetCatalogsAsync<T>() where T : class, IEntityCatalog => _context.Set<T>().ToArrayAsync();
    public Task<Dictionary<int, T>> GetCatalogsDictionaryByIdAsync<T>() where T : class, IEntityCatalog => _context.Set<T>().ToDictionaryAsync(x => x.Id);
    public Task<Dictionary<string, T>> GetCatalogsDictionaryByNameAsync<T>() where T : class, IEntityCatalog => _context.Set<T>().ToDictionaryAsync(x => x.Name);
    public Task<T?> GetCatalogByIdAsync<T>(int id) where T : class, IEntityCatalog => _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
    public Task<T?> GetCatalogByNameAsync<T>(string name) where T : class, IEntityCatalog => _context.Set<T>().FirstOrDefaultAsync(x => x.Name.Equals(name));

    public Task<Guid[]> PrepareProcessableEntityDataAsync<T>(IProcessableEntityStep step, int limit, CancellationToken cToken) where T : class, IProcessableEntity
    {
        throw new NotImplementedException();
    }
    public Task<Guid[]> PrepareProcessableEntityRetryDataAsync<T>(IProcessableEntityStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where T : class, IProcessableEntity
    {
        throw new NotImplementedException();
    }
    public Task<T[]> GetProcessableEntityDataAsync<T>(IProcessableEntityStep step, IEnumerable<Guid> ids, CancellationToken cToken) where T : class, IProcessableEntity
    {
        throw new NotImplementedException();
    }
    public Task SaveProcessableEntityResultAsync<T>(IProcessableEntityStep? step, IEnumerable<T> entities, CancellationToken cToken) where T : class, IProcessableEntity
    {
        throw new NotImplementedException();
    }

    public Task<T?> FindAsync<T>(params object[] id) where T : class, IEntity
    {
        throw new NotImplementedException();
    }
    public Task<T?> FindAsync<T, TId>(TId id) where T : class, IEntity where TId : struct
    {
        throw new NotImplementedException();
    }
}