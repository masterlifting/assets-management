﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Models.Results;
using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Abstractions.Entities.Catalogs;
using Shared.Persistence.Abstractions.Repositories;
using Shared.Persistence.Abstractions.Repositories.BaseParts;
using Shared.Persistence.Contexts;
using Shared.Persistence.Exceptions;

using System.Linq.Expressions;

using static Shared.Persistence.Abstractions.Constants.Enums;

namespace Shared.Persistence.Repositories;

public class PostgreRepository<TEntity, TContext> : IPersistenceSqlRepository<TEntity>
    where TContext : PostgreContext
    where TEntity : class, IPersistentSql
{
    private readonly Lazy<IPersistenceReaderRepository<TEntity>> _reader;
    private readonly Lazy<IPersistenceWriterRepository<TEntity>> _writer;
    public IPersistenceReaderRepository<TEntity> Reader { get => _reader.Value; }
    public IPersistenceWriterRepository<TEntity> Writer { get => _writer.Value; }

    public PostgreRepository(ILogger<TEntity> logger, TContext context)
    {
        var objectId = base.GetHashCode();
        var initiator = $"Postgre repository of '{typeof(TEntity).Name}' by Id {objectId}";

        _reader = new Lazy<IPersistenceReaderRepository<TEntity>>(() => new PostgreReaderRepository<TEntity, TContext>(logger, context, initiator));
        _writer = new Lazy<IPersistenceWriterRepository<TEntity>>(() => new PostgreWriterRepository<TEntity, TContext>(logger, context, initiator));
    }
}
internal class PostgreReaderRepository<TEntity, TContext> : IPersistenceReaderRepository<TEntity>
    where TContext : PostgreContext
    where TEntity : class, IPersistentSql
{
    private readonly ILogger _logger;
    private readonly TContext _context;
    private readonly string _initiator;

    public PostgreReaderRepository(ILogger logger, TContext context, string initiator)
    {
        _logger = logger;
        _context = context;
        _initiator = initiator;
    }

    public Task<TEntity[]> FindManyAsync(Expression<Func<TEntity, bool>> condition) =>
        _context.Set<TEntity>().Where(condition).ToArrayAsync();
    public Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>> condition) =>
        _context.Set<TEntity>().FirstOrDefaultAsync(condition);
    public Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>> condition) =>
        _context.Set<TEntity>().SingleOrDefaultAsync(condition);

    public Task<T[]> GetCatalogsAsync<T>() where T : class, IPersistentCatalog, TEntity => _context.Set<T>().ToArrayAsync();
    public Task<T?> GetCatalogByIdAsync<T>(int id) where T : class, IPersistentCatalog, TEntity => _context.Set<T>().FindAsync(id).AsTask();
    public Task<T?> GetCatalogByNameAsync<T>(string name) where T : class, IPersistentCatalog, TEntity => _context.Set<T>().FirstOrDefaultAsync(x => x.Name.Equals(name));
    public Task<Dictionary<int, T>> GetCatalogsDictionaryByIdAsync<T>() where T : class, IPersistentCatalog, TEntity => _context.Set<T>().ToDictionaryAsync(x => x.Id);
    public Task<Dictionary<string, T>> GetCatalogsDictionaryByNameAsync<T>() where T : class, IPersistentCatalog, TEntity => _context.Set<T>().ToDictionaryAsync(x => x.Name);

    public async Task<T[]> GetProcessableAsync<T>(IProcessStep step, int limit, CancellationToken cToken) where T : class, IPersistentProcess, TEntity
    {
        var tableName = _context.Model.FindEntityType(typeof(T))?.ShortName()
           ?? throw new SharedPersistenceException(typeof(T).Name, "Searching a table name", new("Table name not found"));

        var query = @$"
                UPDATE ""{tableName}"" SET
	                  ""{nameof(IPersistentProcess.ProcessStatusId)}"" = {(int)ProcessStatuses.Processing}
	                , ""{nameof(IPersistentProcess.ProcessAttempt)}"" = ""{nameof(IPersistentProcess.ProcessAttempt)}"" + 1
	                , ""{nameof(IPersistentProcess.Updated)}"" = NOW()
                WHERE ""{nameof(IPersistentProcess.Id)}"" IN 
	                ( SELECT ""{nameof(IPersistentProcess.Id)}""
	                  FROM ""{tableName}""
	                  WHERE ""{nameof(IPersistentProcess.ProcessStepId)}"" = {step.Id} AND ""{nameof(IPersistentProcess.ProcessStatusId)}"" = {(int)ProcessStatuses.Ready} 
	                  LIMIT {limit}
	                  FOR UPDATE SKIP LOCKED )
                RETURNING ""{nameof(IPersistentProcess.Id)}"";";

        var ids = await _context.Set<T>().FromSqlRaw(query).Select(x => x.Id).ToArrayAsync(cToken);

        return await _context.Set<T>().Where(x => ids.Contains(x.Id)).ToArrayAsync(cToken);
    }
    public async Task<T[]> GetUnprocessableAsync<T>(IProcessStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where T : class, IPersistentProcess, TEntity
    {
        var tableName = _context.Model.FindEntityType(typeof(T))?.ShortName()
           ?? throw new SharedPersistenceException(typeof(T).Name, "Searching a table name", new("Table name not found"));

        var query = @$"
                UPDATE ""{tableName}"" SET
	                  ""{nameof(IPersistentProcess.ProcessStatusId)}"" = {(int)ProcessStatuses.Processing}
	                , ""{nameof(IPersistentProcess.ProcessAttempt)}"" = ""{nameof(IPersistentProcess.ProcessAttempt)}"" + 1
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

        var ids = await _context.Set<T>().FromSqlRaw(query).Select(x => x.Id).ToArrayAsync(cToken);

        return await _context.Set<T>().Where(x => ids.Contains(x.Id)).ToArrayAsync(cToken);
    }

}
internal class PostgreWriterRepository<TEntity, TContext> : IPersistenceWriterRepository<TEntity>
    where TContext : PostgreContext
    where TEntity : class, IPersistentSql
{
    private readonly ILogger _logger;
    private readonly TContext _context;
    private readonly string _initiator;

    public PostgreWriterRepository(ILogger logger, TContext context, string initiator)
    {
        _logger = logger;
        _context = context;
        _initiator = initiator;
    }

    public async Task CreateAsync(TEntity entity, CancellationToken? cToken = null)
    {
        await _context.Set<TEntity>().AddAsync(entity, cToken ?? default);
        await _context.SaveChangesAsync(cToken ?? default);

        _logger.LogTrace(_initiator, Constants.Actions.Created, Constants.Actions.Success);
    }
    public async Task CreateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? cToken = null)
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Created, Constants.Actions.NoData);
            return;
        }

        await _context.Set<TEntity>().AddRangeAsync(entities, cToken ?? default);
        var result = await _context.SaveChangesAsync(cToken ?? default);

        _logger.LogTrace(_initiator, Constants.Actions.Created, Constants.Actions.Success, result);
    }
    public async Task<TryResult<TEntity>> TryCreateAsync(TEntity entity, CancellationToken? cToken = null)
    {
        try
        {
            await CreateAsync(entity, cToken);
            return new TryResult<TEntity>(entity);
        }
        catch (Exception exception)
        {
            return new TryResult<TEntity>(exception);
        }
    }
    public async Task<TryResult<TEntity[]>> TryCreateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? cToken = null)
    {
        try
        {
            await CreateRangeAsync(entities, cToken);
            return new TryResult<TEntity[]>(entities.ToArray());
        }
        catch (Exception exception)
        {
            return new TryResult<TEntity[]>(exception);
        }
    }

    public async Task<TEntity[]> UpdateAsync(Expression<Func<TEntity, bool>> condition, TEntity entity, CancellationToken? cToken = null)
    {
        var entities = await _context.Set<TEntity>().Where(condition).ToArrayAsync();

        if (!entities.Any())
            throw new SharedPersistenceException(_initiator, Constants.Actions.Updated, new($"{typeof(TEntity).Name}s by condition '{condition}' not found"));

        if (entities.Length == 1)
        {
            entities[0] = entity;
            _context.Set<TEntity>().Update(entities[0]);
        }
        else
        {
            for (int i = 0; i < entities.Length; i++)
                entities[i] = entity;

            _context.Set<TEntity>().UpdateRange(entities);
        }

        await _context.SaveChangesAsync(cToken ?? default);

        _logger.LogTrace(_initiator, Constants.Actions.Updated, Constants.Actions.Success);

        return entities;
    }
    public async Task<TryResult<TEntity[]>> TryUpdateAsync(Expression<Func<TEntity, bool>> condition, TEntity entity, CancellationToken? cToken = null)
    {
        try
        {
            var entities = await UpdateAsync(condition, entity, cToken);
            return new TryResult<TEntity[]>(entities);
        }
        catch (Exception exception)
        {
            return new TryResult<TEntity[]>(exception);
        }
    }

    public async Task<TEntity[]> DeleteAsync(Expression<Func<TEntity, bool>> condition, CancellationToken? cToken = null)
    {
        var entities = await _context.Set<TEntity>().Where(condition).ToArrayAsync();

        if (!entities.Any())
            throw new SharedPersistenceException(_initiator, Constants.Actions.Deleted, new($"{typeof(TEntity).Name}s by condition '{condition}' not found"));

        _context.Set<TEntity>().RemoveRange(entities);
        await _context.SaveChangesAsync(cToken ?? default);

        _logger.LogTrace(_initiator, Constants.Actions.Deleted, Constants.Actions.Success);

        return entities;
    }
    public async Task<TryResult<TEntity[]>> TryDeleteAsync(Expression<Func<TEntity, bool>> condition, CancellationToken? cToken = null)
    {
        try
        {
            var entities = await DeleteAsync(condition, cToken);
            return new TryResult<TEntity[]>(entities);
        }
        catch (Exception exception)
        {
            return new TryResult<TEntity[]>(exception);
        }
    }

    public async Task SaveProcessableAsync<T>(IProcessStep? step, IEnumerable<T> entities, CancellationToken cToken) where T : class, IPersistentProcess, TEntity
    {
        var array = entities.ToArray();

        if (step is not null)
            foreach (var entity in array.Where(x => x.ProcessStatusId != (int)ProcessStatuses.Error))
                entity.ProcessStepId = step.Id;

        try
        {
            await SetTransactionAsync(cToken);

            foreach (var item in array)
                await UpdateAsync(x => true, item);
        }
        catch (Exception exception)
        {
            throw new SharedPersistenceException("PostgreWriterRepository", nameof(SaveProcessableAsync), new(exception));
        }
        finally
        {
            await CommitTransactionAsync(cToken);
        }
    }

    public Task SetTransactionAsync(CancellationToken? cToken = null) => _context.Database.BeginTransactionAsync(cToken ?? default);
    public Task CommitTransactionAsync(CancellationToken? cToken = null) => _context.Database.CommitTransactionAsync(cToken ?? default);
}