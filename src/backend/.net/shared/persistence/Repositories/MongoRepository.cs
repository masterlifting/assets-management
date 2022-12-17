﻿using Microsoft.Extensions.Logging;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

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

public class MongoRepository<TEntity, TContext> : IPersistenceNoSqlRepository<TEntity>
    where TContext : MongoContext
    where TEntity : class, IPersistentNoSql
{
    private readonly Lazy<IPersistenceReaderRepository<TEntity>> _reader;
    private readonly Lazy<IPersistenceWriterRepository<TEntity>> _writer;

    public IPersistenceReaderRepository<TEntity> Reader { get => _reader.Value; }
    public IPersistenceWriterRepository<TEntity> Writer { get => _writer.Value; }

    public MongoRepository(ILogger<TEntity> logger, TContext context)
    {
        var objectId = base.GetHashCode();
        var initiator = $"Mongo repository of '{typeof(TEntity).Name}' by Id {objectId}";

        _reader = new Lazy<IPersistenceReaderRepository<TEntity>>(() => new MongoReaderRepository<TEntity, TContext>(logger, context, initiator));
        _writer = new Lazy<IPersistenceWriterRepository<TEntity>>(() => new MongoWriterRepository<TEntity, TContext>(logger, context, initiator));
    }
}
internal class MongoReaderRepository<TEntity, TContext> : IPersistenceReaderRepository<TEntity>
    where TContext : MongoContext
    where TEntity : class, IPersistentNoSql
{
    private readonly ILogger _logger;
    private readonly TContext _context;
    private readonly string _initiator;

    public MongoReaderRepository(ILogger logger, TContext context, string initiator)
    {
        _logger = logger;
        _context = context;
        _initiator = initiator;
    }

    public Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>> condition) =>
        _context.Set<TEntity>().SingleOrDefaultAsync(condition);
    public Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>> condition) =>
        _context.Set<TEntity>().FirstOrDefaultAsync(condition);
    public Task<TEntity[]> FindManyAsync(Expression<Func<TEntity, bool>> condition) =>
        Task.Run(() => _context.Set<TEntity>().Where(condition).ToArray());

    public Task<T[]> GetCatalogsAsync<T>() where T : class, IPersistentCatalog, TEntity =>
        Task.Run(() => _context.Set<T>().ToArray());
    public Task<T?> GetCatalogByIdAsync<T>(int id) where T : class, IPersistentCatalog, TEntity =>
        Task.Run(() => _context.Set<T>().FirstOrDefault(x => x.Id == id));
    public Task<T?> GetCatalogByNameAsync<T>(string name) where T : class, IPersistentCatalog, TEntity =>
        Task.Run(() => _context.Set<T>().FirstOrDefault(x => x.Name.Equals(name)));
    public Task<Dictionary<int, T>> GetCatalogsDictionaryByIdAsync<T>() where T : class, IPersistentCatalog, TEntity =>
            Task.Run(() => _context.Set<T>().ToDictionary(x => x.Id));
    public Task<Dictionary<string, T>> GetCatalogsDictionaryByNameAsync<T>() where T : class, IPersistentCatalog, TEntity =>
            Task.Run(() => _context.Set<T>().ToDictionary(x => x.Name));

    public async Task<T[]> GetProcessableAsync<T>(IProcessStep step, int limit, CancellationToken cToken) where T : class, IPersistentProcess, TEntity
    {
        var collection = _context.Get<T>();

        var targetUpdatedDateTime = DateTime.UtcNow;
        var targetStatusId = (int)ProcessStatuses.Processing;

        var updateBuilder = Builders<T>.Update
            .Set(x => x.Updated, targetUpdatedDateTime)
            .Set(x => x.ProcessStatusId, targetStatusId)
            .Inc(x => x.ProcessAttempt, 1);

        Expression<Func<T, bool>> updateFilter = x =>
            x.ProcessStepId == step.Id
            && x.ProcessStatusId == (int)ProcessStatuses.Ready;

        List<T> results = new(limit);

        foreach (var _ in Enumerable.Range(0, limit))
        {
            var updatedResult = await collection.FindOneAndUpdateAsync(updateFilter, updateBuilder, null, cToken);

            if (updatedResult is null)
                break;

            results.Add(updatedResult);
        }

        return results.ToArray();
    }
    public async Task<T[]> GetUnprocessableAsync<T>(IProcessStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where T : class, IPersistentProcess, TEntity
    {
        var collection = _context.Get<T>();

        var targetUpdatedDateTime = DateTime.UtcNow;
        var targetStatusId = (int)ProcessStatuses.Processing;

        var updateBuilder = Builders<T>.Update
            .Set(x => x.Updated, targetUpdatedDateTime)
            .Set(x => x.ProcessStatusId, targetStatusId)
            .Inc(x => x.ProcessAttempt, 1);

        Expression<Func<T, bool>> updateFilter = x =>
            x.ProcessStepId == step.Id
            && ((x.ProcessStatusId == (int)ProcessStatuses.Processing && x.Updated < updateTime) || (x.ProcessStatusId == (int)ProcessStatuses.Error))
            && (x.ProcessAttempt < maxAttempts);

        List<T> results = new(limit);

        foreach (var _ in Enumerable.Range(0, limit))
        {
            var updatedResult = await collection.FindOneAndUpdateAsync(updateFilter, updateBuilder, null, cToken);

            if (updatedResult is null)
                break;

            results.Add(updatedResult);
        }

        return results.ToArray();
    }

}
internal class MongoWriterRepository<TEntity, TContext> : IPersistenceWriterRepository<TEntity>
    where TContext : MongoContext
    where TEntity : class, IPersistentNoSql
{
    private readonly ILogger _logger;
    private readonly TContext _context;
    private readonly string _initiator;

    public MongoWriterRepository(ILogger logger, TContext context, string initiator)
    {
        _logger = logger;
        _context = context;
        _initiator = initiator;
    }

    public async Task CreateAsync(TEntity entity, CancellationToken? cToken = null)
    {
        await _context.CreateAsync(entity, cToken ?? default);
        _logger.LogTrace(_initiator, typeof(TEntity).Name + ' ' + Constants.Actions.Created, Constants.Actions.Success);

    }
    public async Task CreateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? cToken = null)
    {
        if (!entities.Any())
        {
            _logger.LogTrace(_initiator, Constants.Actions.Created, Constants.Actions.NoData);
            return;
        }

        await _context.CreateRangeAsync(entities, cToken ?? default);

        _logger.LogTrace(_initiator, Constants.Actions.Created, Constants.Actions.Success);
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
        var entities = await _context.UpdateAsync(condition, entity, cToken ?? default);
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
        var entities = await _context.DeleteAsync(condition, cToken ?? default);

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
            throw new SharedPersistenceException("MongoWriterRepository", nameof(SaveProcessableAsync), new(exception));
        }
        finally
        {
            await CommitTransactionAsync(cToken);
        }
    }

    public Task SetTransactionAsync(CancellationToken? cToken = null) => _context.SetTransactionAsync(cToken ?? default);
    public Task CommitTransactionAsync(CancellationToken? cToken = null) => _context.CommitTransactionAsync(cToken ?? default);
}