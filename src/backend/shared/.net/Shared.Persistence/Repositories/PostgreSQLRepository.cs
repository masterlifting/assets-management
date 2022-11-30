using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Contracts.Models.Results;
using Shared.Extensions.Logging;
using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Abstractions.Entities.Catalogs;
using Shared.Persistence.Abstractions.Repositories;
using Shared.Persistence.Contexts;
using Shared.Persistence.Exceptions;

using static Shared.Persistence.Abstractions.Constants.Enums;

namespace Shared.Persistence.Repositories;

public class PostgreSQLRepository<TContext> : IPostgreSQLRepository where TContext : PostgreSQLContext
{
    private readonly string _initiator;
    private readonly ILogger _logger;
    private readonly TContext _context;

    public PostgreSQLRepository(ILogger<PostgreSQLRepository<TContext>> logger, TContext context)
    {
        _logger = logger;
        _context = context;
        var objectId = base.GetHashCode();
        _initiator = $"{nameof(PostgreSQLRepository<TContext>)} ({objectId})";
    }

    public IQueryable<T> Set<T>() where T : class, IPersistent => _context.Set<T>();

    public virtual async Task CreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IPersistent
    {
        if (!cToken.HasValue)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            await _context.Set<T>().AddAsync(entity, cToken.Value);
            await _context.SaveChangesAsync(cToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Create, Constants.Actions.Success);
    }
    public virtual async Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Create, Constants.Actions.NoData);
            return;
        }

        int result;
        if (!cToken.HasValue)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            result = await _context.SaveChangesAsync();
        }
        else
        {
            await _context.Set<T>().AddRangeAsync(entities, cToken.Value);
            result = await _context.SaveChangesAsync(cToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Create, Constants.Actions.Success, result);
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

    public virtual async Task UpdateAsync<T>(object[] id, T entity, CancellationToken? cToken = null) where T : class, IPersistent
    {
        if (await _context.Set<T>().FindAsync(id) is null)
            throw new SharedPersistenceException(_initiator, Constants.Actions.Update, new($"Entity by Id: '{id}' not found"));

        if (!cToken.HasValue)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync(cToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.Success);
    }
    public virtual async Task UpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.NoData);
            return;
        }

        int result;
        if (!cToken.HasValue)
        {
            _context.Set<T>().UpdateRange(entities);
            result = await _context.SaveChangesAsync();
        }
        else
        {
            _context.Set<T>().UpdateRange(entities);
            result = await _context.SaveChangesAsync(cToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.Success, result);
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
        var entity = await _context.Set<T>().FindAsync(id);

        if (entity is null)
            throw new SharedPersistenceException(_initiator, Constants.Actions.Update, new($"Entity by Id: '{id}' not found"));

        if (!cToken.HasValue)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(cToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Delete, Constants.Actions.Success);

        return entity;
    }
    public virtual async Task DeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IPersistent
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Update, Constants.Actions.NoData);
            return;
        }

        int result;
        if (!cToken.HasValue)
        {
            _context.Set<T>().RemoveRange(entities);
            result = await _context.SaveChangesAsync();
        }
        else
        {
            _context.Set<T>().RemoveRange(entities);
            result = await _context.SaveChangesAsync(cToken.Value);
        }

        _logger.LogTrace(_initiator, Constants.Actions.Delete, Constants.Actions.Success, result);
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

    public Task<T[]> GetCatalogsAsync<T>() where T : class, IPersistentCatalog => _context.Set<T>().ToArrayAsync();
    public Task<Dictionary<int, T>> GetCatalogsDictionaryByIdAsync<T>() where T : class, IPersistentCatalog => _context.Set<T>().ToDictionaryAsync(x => x.Id);
    public Task<Dictionary<string, T>> GetCatalogsDictionaryByNameAsync<T>() where T : class, IPersistentCatalog => _context.Set<T>().ToDictionaryAsync(x => x.Name);
    public Task<T?> GetCatalogByIdAsync<T>(int id) where T : class, IPersistentCatalog => _context.Set<T>().FindAsync(id).AsTask();
    public Task<T?> GetCatalogByNameAsync<T>(string name) where T : class, IPersistentCatalog => _context.Set<T>().FirstOrDefaultAsync(x => x.Name.Equals(name));

    public async Task<Guid[]> GetPreparedProcessableIdsAsync<T>(IProcessStep step, int limit, CancellationToken cToken) where T : class, IPersistentProcess
    {
        var tableName = _context.Model.FindEntityType(typeof(T))?.ShortName()
            ?? throw new SharedPersistenceException(typeof(T).Name, "Searching a table name", new("Table name not found"));

        var query = @$"
                UPDATE ""{tableName}"" SET
	                  ""{nameof(IPersistentProcess.ProcessStatusId)}"" = {(int)ProcessStatuses.Processing}
	                , ""{nameof(IPersistentProcess.ProcessAttempt)}"" = CASE WHEN ""{nameof(IPersistentProcess.ProcessStepId)}"" = {step.Id} THEN ""{nameof(IPersistentProcess.ProcessAttempt)}"" + 1 ELSE ""{nameof(IPersistentProcess.ProcessAttempt)}"" END
	                , ""{nameof(IPersistentProcess.Updated)}"" = NOW()
                WHERE ""{nameof(IPersistentProcess.Id)}"" IN 
	                ( SELECT ""{nameof(IPersistentProcess.Id)}""
	                  FROM ""{tableName}""
	                  WHERE ""{nameof(IPersistentProcess.ProcessStepId)}"" = {step.Id} AND ""{nameof(IPersistentProcess.ProcessStatusId)}"" = {(int)ProcessStatuses.Ready} 
	                  LIMIT {limit}
	                  FOR UPDATE SKIP LOCKED )
                RETURNING ""{nameof(IPersistentProcess.Id)}"";";

        var result = await _context.GuidIds.FromSqlRaw(query).ToArrayAsync(cToken);

        return result.Select(x => x.Id).ToArray();
    }
    public async Task<Guid[]> GetPrepareUnprocessableIdsAsync<T>(IProcessStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where T : class, IPersistentProcess
    {
        var tableName = _context.Model.FindEntityType(typeof(T))?.ShortName()
            ?? throw new SharedPersistenceException(typeof(T).Name, "Searching a table name", new("Table name not found"));

        var query = @$"
                UPDATE ""{tableName}"" SET
	                  ""{nameof(IPersistentProcess.ProcessStatusId)}"" = {(int)ProcessStatuses.Processing}
	                , ""{nameof(IPersistentProcess.ProcessAttempt)}"" = CASE WHEN ""{nameof(IPersistentProcess.ProcessStepId)}"" = {step.Id} THEN ""{nameof(IPersistentProcess.ProcessAttempt)}"" + 1 ELSE ""{nameof(IPersistentProcess.ProcessAttempt)}"" END
	                , ""{nameof(IPersistentProcess.Updated)}"" = NOW()
                WHERE ""{nameof(IPersistentProcess.Id)}"" IN 
	                ( SELECT ""{nameof(IPersistentProcess.Id)}""
	                  FROM ""{tableName}""
	                  WHERE 
                            ""{nameof(IPersistentProcess.ProcessStepId)}"" = {step.Id} 
                            AND ((""{nameof(IPersistentProcess.ProcessStatusId)}"" = {(int)ProcessStatuses.Processing} AND ""{nameof(IPersistentProcess.Updated)}"" < '{updateTime: yyyy-MM-dd HH:mm:ss}') OR (""{nameof(IPersistentProcess.ProcessStatusId)}"" = {(int)ProcessStatuses.Error}))
			                AND ""{nameof(IPersistentProcess.ProcessAttempt)}"" < {maxAttempts}
	                  LIMIT {limit}
	                  FOR UPDATE SKIP LOCKED )
                RETURNING ""{nameof(IPersistentProcess.Id)}"";";

        var result = await _context.GuidIds.FromSqlRaw(query).ToArrayAsync(cToken);

        return result.Select(x => x.Id).ToArray();
    }
    public Task<T[]> GetProcessableEntitiesAsync<T>(IProcessStep step, IEnumerable<Guid> ids, CancellationToken cToken) where T : class, IPersistentProcess =>
    _context.Set<T>().Where(x => x.ProcessStepId == step.Id && ids.Contains(x.Id)).ToArrayAsync(cToken);
    public Task SaveProcessableEntitiesAsync<T>(IProcessStep? step, IEnumerable<T> entities, CancellationToken cToken) where T : class, IPersistentProcess
    {
        var array = entities.ToArray();

        if (step is null)
            return UpdateRangeAsync(array, cToken);

        foreach (var entity in array.Where(x => x.ProcessStatusId != (int)ProcessStatuses.Error))
            entity.ProcessStepId = step.Id;

        return UpdateRangeAsync(array, cToken);
    }

    public Task<T?> FindAsync<T>(params object[] id) where T : class, IPersistent => _context.Set<T>().FindAsync(id).AsTask();
    public Task<T?> FindAsync<T, TId>(TId id) where T : class, IPersistent where TId : struct => _context.Set<T>().FindAsync(id).AsTask();
}
