using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        _logger.LogDebug(_initiator, Constants.Create, Constants.Success);
    }
    public virtual async Task CreateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? ctToken = null)
    {
        if (!entities.Any())
        {
            _logger.LogDebug(_initiator, Constants.Create, Constants.NoData);
            return;
        }

        int result;
        if (!ctToken.HasValue)
        {
            await DbSet.AddRangeAsync(entities).ConfigureAwait(false);
            result = await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            await DbSet.AddRangeAsync(entities, ctToken.Value).ConfigureAwait(false);
            result = await _context.SaveChangesAsync(ctToken.Value).ConfigureAwait(false);
        }

        _logger.LogDebug(_initiator, Constants.Create, Constants.Success, result);
    }

    public virtual async Task UpdateAsync(object[] id, TEntity entity, CancellationToken? ctToken = null)
    {
        if (await DbSet.FindAsync(id) is null)
            throw new PersistenseEntityException(_initiator, Constants.Update, $"Entity by Id: '{id}' not found");

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

        _logger.LogDebug(_initiator, Constants.Update, Constants.Success);
    }
    public virtual async Task UpdateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? ctToken = null)
    {
        if (!entities.Any())
        {
            _logger.LogDebug(_initiator, Constants.Update, Constants.NoData);
            return;
        }

        int result;
        if (!ctToken.HasValue)
        {
            DbSet.UpdateRange(entities);
            result = await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            DbSet.UpdateRange(entities);
            result = await _context.SaveChangesAsync(ctToken.Value).ConfigureAwait(false);
        }

        _logger.LogDebug(_initiator, Constants.Update, Constants.Success, result);
    }

    public virtual async Task<TEntity> DeleteAsync(object[] id, CancellationToken? ctToken = null)
    {
        var entity = await DbSet.FindAsync(id);

        if (entity is null)
            throw new PersistenseEntityException(_initiator, Constants.Update, $"Entity by Id: '{id}' not found");

        if (!ctToken.HasValue)
        {
            DbSet.Remove(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            DbSet.Remove(entity);
            await _context.SaveChangesAsync(ctToken.Value).ConfigureAwait(false);
        }

        _logger.LogDebug(_initiator, Constants.Delete, Constants.Success);

        return entity;
    }
    public virtual async Task DeleteRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? ctToken = null)
    {
        if (!entities.Any())
        {
            _logger.LogDebug(_initiator, Constants.Update, Constants.NoData);
            return;
        }

        int result;
        if (!ctToken.HasValue)
        {
            DbSet.RemoveRange(entities);
            result = await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            DbSet.RemoveRange(entities);
            result = await _context.SaveChangesAsync(ctToken.Value).ConfigureAwait(false);
        }

        _logger.LogDebug(_initiator, Constants.Delete, Constants.Success, result);
    }

    public Task<bool> TryCreateAsync(TEntity entity, CancellationToken? ctToken = null, out string error)
    {
        error = string.Empty;
        try
        {
            return Task.FromResult(CreateAsync(entity, ctToken).IsCompletedSuccessfully);
        }
        catch (Exception exception)
        {
            error = exception.InnerException?.Message ?? exception.Message;
            return Task.FromResult(false);
        }
    }

    public Task<bool> TryCreateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? ctToken = null, out string error)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryUpdateAsync(object[] id, TEntity entity, CancellationToken? ctToken = null, out string error)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryUpdateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? ctToken = null, out string error)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryDeleteAsync(object[] id, CancellationToken? ctToken = null, out string error)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryDeleteRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? ctToken = null, out string error)
    {
        throw new NotImplementedException();
    }
}