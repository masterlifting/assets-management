using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Contracts.Models.Results;
using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Exceptions;

namespace Shared.Persistense.Repositories;

public class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : class, IEntity
    where TContext : DbContext
{
    public DbSet<TEntity> DbSet { get; }

    private readonly string _initiator;
    private readonly TContext _context;
    private readonly ILogger _logger;

    protected Repository(ILogger logger, TContext context)
    {
        _context = context;

        _logger = logger;
        DbSet = context.Set<TEntity>();

        var objectId = base.GetHashCode();
        _initiator = $"Repository ({typeof(TEntity).Name}) {objectId}";
    }

    public virtual async Task CreateAsync(TEntity entity, CancellationToken? ctToken = null)
    {
        if (!ctToken.HasValue)
        {
            await DbSet.AddAsync(entity).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            await DbSet.AddAsync(entity, ctToken.Value).ConfigureAwait(false);
            await _context.SaveChangesAsync(ctToken.Value).ConfigureAwait(false);
        }

        _logger.LogDebug(_initiator, Constants.Actions.Create, Constants.Actions.Success);
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
            _logger.LogDebug(_initiator, Constants.Actions.Create, Constants.Actions.NoData);
            return;
        }

        int result;
        if (!cToken.HasValue)
        {
            await DbSet.AddRangeAsync(entities).ConfigureAwait(false);
            result = await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            await DbSet.AddRangeAsync(entities, cToken.Value).ConfigureAwait(false);
            result = await _context.SaveChangesAsync(cToken.Value).ConfigureAwait(false);
        }

        _logger.LogDebug(_initiator, Constants.Actions.Create, Constants.Actions.Success, result);
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
        if (await DbSet.FindAsync(id) is null)
            throw new SharedPersistenseEntityException(_initiator, Constants.Actions.Update, $"Entity by Id: '{id}' not found");

        if (!ctToken.HasValue)
        {
            DbSet.Update(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            DbSet.Update(entity);
            await _context.SaveChangesAsync(ctToken.Value).ConfigureAwait(false);
        }

        _logger.LogDebug(_initiator, Constants.Actions.Update, Constants.Actions.Success);
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
            _logger.LogDebug(_initiator, Constants.Actions.Update, Constants.Actions.NoData);
            return;
        }

        int result;
        if (!cToken.HasValue)
        {
            DbSet.UpdateRange(entities);
            result = await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            DbSet.UpdateRange(entities);
            result = await _context.SaveChangesAsync(cToken.Value).ConfigureAwait(false);
        }

        _logger.LogDebug(_initiator, Constants.Actions.Update, Constants.Actions.Success, result);
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
        var entity = await DbSet.FindAsync(id);

        if (entity is null)
            throw new SharedPersistenseEntityException(_initiator, Constants.Actions.Update, $"Entity by Id: '{id}' not found");

        if (!cToken.HasValue)
        {
            DbSet.Remove(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            DbSet.Remove(entity);
            await _context.SaveChangesAsync(cToken.Value).ConfigureAwait(false);
        }

        _logger.LogDebug(_initiator, Constants.Actions.Delete, Constants.Actions.Success);

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
            _logger.LogDebug(_initiator, Constants.Actions.Update, Constants.Actions.NoData);
            return;
        }

        int result;
        if (!cToken.HasValue)
        {
            DbSet.RemoveRange(entities);
            result = await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            DbSet.RemoveRange(entities);
            result = await _context.SaveChangesAsync(cToken.Value).ConfigureAwait(false);
        }

        _logger.LogDebug(_initiator, Constants.Actions.Delete, Constants.Actions.Success, result);
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