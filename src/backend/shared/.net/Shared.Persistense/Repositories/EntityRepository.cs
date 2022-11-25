using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Contracts.Models.Results;
using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Exceptions;

namespace Shared.Persistense.Repositories;

public class EntityRepository<TEntity, TContext> : IEntityRepository<TEntity> where TEntity : class, IEntity
    where TContext : DbContext
{
    private readonly string _initiator;
    private readonly TContext _context;
    private readonly ILogger _logger;

    protected EntityRepository(ILogger logger, TContext context)
    {
        _context = context;
        _logger = logger;

        var objectId = base.GetHashCode();
        _initiator = $"{typeof(TEntity).Name} repository ({objectId})";
    }

    public virtual async Task CreateAsync(TEntity entity, CancellationToken? ctToken = null)
    {
        if (!ctToken.HasValue)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            await _context.Set<TEntity>().AddAsync(entity, ctToken.Value);
            await _context.SaveChangesAsync(ctToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Create, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryCreateAsync(TEntity entity, CancellationToken? cToken = null)
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
    public virtual async Task CreateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? cToken = null)
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Create, Constants.Actions.NoData);
            return;
        }

        int result;
        if (!cToken.HasValue)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
            result = await _context.SaveChangesAsync();
        }
        else
        {
            await _context.Set<TEntity>().AddRangeAsync(entities, cToken.Value);
            result = await _context.SaveChangesAsync(cToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Create, Constants.Actions.Success, result);
    }
    public virtual async Task<Result> TryCreateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? cToken = null)
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

    public virtual async Task UpdateAsync(object[] id, TEntity entity, CancellationToken? ctToken = null)
    {
        if (await _context.Set<TEntity>().FindAsync(id) is null)
            throw new SharedPersistenseEntityException(_initiator, Constants.Actions.Update, new($"Entity by Id: '{id}' not found"));

        if (!ctToken.HasValue)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync(ctToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.Success);
    }
    public virtual async Task<Result> TryUpdateAsync(object[] id, TEntity entity, CancellationToken? cToken = null)
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
    public virtual async Task UpdateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? cToken = null)
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.NoData);
            return;
        }

        int result;
        if (!cToken.HasValue)
        {
            _context.Set<TEntity>().UpdateRange(entities);
            result = await _context.SaveChangesAsync();
        }
        else
        {
            _context.Set<TEntity>().UpdateRange(entities);
            result = await _context.SaveChangesAsync(cToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.Success, result);
    }
    public virtual async Task<Result> TryUpdateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? cToken = null)
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

    public virtual async Task<TEntity> DeleteAsync(object[] id, CancellationToken? cToken = null)
    {
        var entity = await _context.Set<TEntity>().FindAsync(id);

        if (entity is null)
            throw new SharedPersistenseEntityException(_initiator, Constants.Actions.Update, new($"Entity by Id: '{id}' not found"));

        if (!cToken.HasValue)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync(cToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Delete, Constants.Actions.Success);

        return entity;
    }
    public virtual async Task<ResultData<TEntity>> TryDeleteAsync(object[] id, CancellationToken? cToken = null)
    {
        try
        {
            var entity = await DeleteAsync(id, cToken);
            return new ResultData<TEntity>(new(true), entity);
        }
        catch (Exception exception)
        {
            return new ResultData<TEntity>(new(false, exception.InnerException?.Message ?? exception.Message), null);
        }
    }
    public virtual async Task DeleteRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? cToken = null)
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.NoData);
            return;
        }

        int result;
        if (!cToken.HasValue)
        {
            _context.Set<TEntity>().RemoveRange(entities);
            result = await _context.SaveChangesAsync();
        }
        else
        {
            _context.Set<TEntity>().RemoveRange(entities);
            result = await _context.SaveChangesAsync(cToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Delete, Constants.Actions.Success, result);
    }
    public virtual async Task<Result> TryDeleteRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? cToken = null)
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
}