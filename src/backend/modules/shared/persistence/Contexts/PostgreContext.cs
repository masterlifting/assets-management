﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

using Shared.Persistence.Abstractions.Contexts;
using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Settings.Connections;

using SharpCompress.Common;
using static Shared.Persistence.Abstractions.Constants.Enums;

namespace Shared.Persistence.Contexts;

public abstract class PostgreContext : DbContext, IPostgrePersistenceContext
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly PostgreSQLConnectionSettings _connectionSettings;
    protected PostgreContext(ILoggerFactory loggerFactory, PostgreSQLConnectionSettings connectionSettings)
    {
        _loggerFactory = loggerFactory;
        _connectionSettings = connectionSettings;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseLoggerFactory(_loggerFactory);
        builder.UseNpgsql(_connectionSettings.GetConnectionString());
        base.OnConfiguring(builder);
    }

    public new IQueryable<T> Set<T>() where T : class, IPersistentSql => base.Set<T>();

    public string GetTableName<T>() where T : class, IPersistentSql
    {
        var name =  Model.FindEntityType(typeof(T))?.ShortName();
        return name ?? throw new NullReferenceException("Table name was not found");
    }

    public Task<T?> FindByIdAsync<T>(CancellationToken cToken, object[] id) where T : class, IPersistentSql =>
        base.Set<T>().FindAsync(id, cToken).AsTask();
    public Task<T[]> FindManyAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cToken = default) where T : class, IPersistentSql =>
        Set<T>().Where(condition).ToArrayAsync(cToken);
    public Task<T?> FindFirstAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cToken = default) where T : class, IPersistentSql =>
        Set<T>().FirstOrDefaultAsync(condition, cToken);
    public Task<T?> FindSingleAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cToken = default) where T : class, IPersistentSql =>
        Set<T>().SingleOrDefaultAsync(condition, cToken);

    public async Task CreateAsync<T>(T entity, CancellationToken cToken = default) where T : class, IPersistentSql
    {
        await base.Set<T>().AddAsync(entity, cToken);
        await SaveChangesAsync(cToken);
    }
    public async Task CreateManyAsync<T>(IReadOnlyCollection<T> entities, CancellationToken cToken = default) where T : class, IPersistentSql
    {
        await base.Set<T>().AddRangeAsync(entities, cToken);
        await SaveChangesAsync(cToken);
    }
    public async Task<T[]> UpdateAsync<T>(Expression<Func<T, bool>> condition, T entity, CancellationToken cToken = default) where T : class, IPersistentSql
    {
        var entities = await FindManyAsync(condition, cToken);

        if (!entities.Any())
            return entities;

        var entityProperties = typeof(T).GetProperties();
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
            base.Set<T>().Update(entities[0]);
        else
            base.Set<T>().UpdateRange(entities);

        await SaveChangesAsync(cToken);

        return entities;
    }
    public async Task<T[]> DeleteAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cToken = default) where T : class, IPersistentSql
    {
        var entities = await FindManyAsync(condition, cToken);

        if (!entities.Any())
            return entities;

        base.Set<T>().RemoveRange(entities);

        await SaveChangesAsync(cToken);

        return entities;
    }
    public Task<T[]> ExecuteQueryAsync<T>(string query, CancellationToken cToken = default) where T : class, IPersistentSql => 
        base.Set<T>().FromSqlRaw(query).ToArrayAsync(cToken);
    public Task StartTransactionAsync(CancellationToken cToken = default) => Database.BeginTransactionAsync(cToken);
    public Task CommitTransactionAsync(CancellationToken cToken = default) => Database.CommitTransactionAsync(cToken);
    public Task RollbackTransactionAsync(CancellationToken cToken = default) => Database.RollbackTransactionAsync(cToken);
}
