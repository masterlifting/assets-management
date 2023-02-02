using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistence.Abstractions.Contexts;
using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Settings.Connections;

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

    public IQueryable<IPersistentSql> Set() => Set<IPersistentSql>();

    public Task<IPersistentSql?> FindFirstAsync(Expression<Func<IPersistentSql, bool>> condition, CancellationToken cToken = default) =>
        Set().FirstOrDefaultAsync(condition, cToken);
    public Task<IPersistentSql[]> FindManyAsync(Expression<Func<IPersistentSql, bool>> condition, CancellationToken cToken = default) =>
        Set().Where(condition).ToArrayAsync(cToken);
    public Task<IPersistentSql?> FindSingleAsync(Expression<Func<IPersistentSql, bool>> condition, CancellationToken cToken = default) =>
        Set().SingleOrDefaultAsync(condition, cToken);

    public async Task CreateAsync(IPersistentSql entity, CancellationToken cToken = default)
    {
        await Set<IPersistentSql>().AddAsync(entity, cToken);
    }

    public async Task<IPersistentSql[]> UpdateAsync(Expression<Func<IPersistentSql, bool>> condition, IPersistentSql entity, CancellationToken cToken = default)
    {
        var entities = await FindManyAsync(condition, cToken);

        if (!entities.Any())
            return entities;

        var entityProperties = typeof(IPersistentSql).GetProperties();
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
            Set<IPersistentSql>().Update(entities[0]);
        else
            Set<IPersistentSql>().UpdateRange(entities);

        await SaveChangesAsync(cToken);

        return entities;
    }
    public async Task<IPersistentSql[]> DeleteAsync(Expression<Func<IPersistentSql, bool>> condition, CancellationToken cToken = default)
    {
        var entities = await FindManyAsync(condition, cToken);

        if (!entities.Any())
            return entities;

        Set<IPersistentSql>().RemoveRange(entities);

        await SaveChangesAsync(cToken);

        return entities;
    }

    public Task SetTransactionAsync(CancellationToken cToken = default)
    {
        throw new NotImplementedException();
    }
    public Task CommitTransactionAsync(CancellationToken cToken = default)
    {
        throw new NotImplementedException();
    }
    public Task RollbackTransactionAsync(CancellationToken cToken = default)
    {
        throw new NotImplementedException();
    }
}
