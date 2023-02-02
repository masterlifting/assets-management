using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Extensions.Logging;
using Shared.Models.Results;
using Shared.Persistence.Abstractions.Contexts;
using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Abstractions.Entities.Catalogs;
using Shared.Persistence.Abstractions.Repositories;
using Shared.Persistence.Abstractions.Repositories.Parts;
using Shared.Persistence.Exceptions;

using System.Linq.Expressions;

using static Shared.Persistence.Abstractions.Constants.Enums;

namespace Shared.Persistence.Repositories;

public abstract class PostgreRepository<TEntity> : IPersistenceSqlRepository<TEntity>
    where TEntity : class, IPersistentSql
{
    private readonly Lazy<IPersistenceReaderRepository<TEntity>> _reader;
    private readonly Lazy<IPersistenceWriterRepository<TEntity>> _writer;

    public IPersistenceReaderRepository<TEntity> Reader { get => _reader.Value; }
    public IPersistenceWriterRepository<TEntity> Writer { get => _writer.Value; }

    protected PostgreRepository(ILogger<TEntity> logger, IPostgrePersistenceContext context)
    {
        var objectId = base.GetHashCode();
        var initiator = $"Postgre repository of '{typeof(TEntity).Name}' by Id {objectId}";

        _reader = new Lazy<IPersistenceReaderRepository<TEntity>>(() => new PostgreReaderRepository<TEntity>(context));
        _writer = new Lazy<IPersistenceWriterRepository<TEntity>>(() => new PostgreWriterRepository<TEntity>(logger, context, initiator));
    }
}
internal sealed class PostgreReaderRepository<TEntity> : IPersistenceReaderRepository<TEntity> where TEntity : class, IPersistentSql
{
    private readonly IPostgrePersistenceContext _context;
    public PostgreReaderRepository(IPostgrePersistenceContext context) => _context = context;

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
internal sealed class PostgreWriterRepository<TEntity> : IPersistenceWriterRepository<TEntity> where TEntity : class, IPersistentSql
{
    private readonly ILogger _logger;
    private readonly IPostgrePersistenceContext _context;
    private readonly string _initiator;

    public PostgreWriterRepository(ILogger logger, IPostgrePersistenceContext context, string initiator)
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
        {
            _logger.LogTrace(_initiator, Constants.Actions.Updated, Constants.Actions.Success, $"{typeof(TEntity).Name}s by condition '{condition}' not found");
            return entities;
        }

        var entityProperties = typeof(TEntity).GetProperties();
        var entityPropertiesDictionary = entityProperties.ToDictionary(x => x.Name, x => x.GetValue(entity));

        for (int i = 0; i < entities.Length; i++)
        {
            for (int j = 0; j < entityProperties.Length; j++)
            {
                var newValue = entityPropertiesDictionary[entityProperties[j].Name];

                if (newValue == default)
                    continue;

                var oldValue = entityProperties[j].GetValue(entities[i]);

                if (oldValue != newValue)
                    entityProperties[j].SetValue(entities[i], newValue);
            }
        }

        if (entities.Length == 1)
            _context.Set<TEntity>().Update(entities[0]);
        else
            _context.Set<TEntity>().UpdateRange(entities);

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
        {
            _logger.LogTrace(_initiator, Constants.Actions.Deleted, Constants.Actions.Success, $"{typeof(TEntity).Name}s by condition '{condition}' not found");
            return entities;
        }

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
        try
        {
            foreach (var entity in entities)
            {
                entity.Updated = DateTime.UtcNow;

                if (entity.ProcessStatusId != (int)ProcessStatuses.Error)
                {
                    entity.Error = null;

                    if (step is not null)
                        entity.ProcessStepId = step.Id;
                }

                _context.Update(entity);
            }

            await _context.SaveChangesAsync(cToken);

            _logger.LogTrace(_initiator, Constants.Actions.Updated, Constants.Actions.Success);
        }
        catch (Exception exception)
        {
            throw new SharedPersistenceException("PostgreWriterRepository", nameof(SaveProcessableAsync), new(exception));
        }
    }
}
