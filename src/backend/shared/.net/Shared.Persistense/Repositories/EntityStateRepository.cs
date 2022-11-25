using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;
using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Exceptions;

using static Shared.Persistense.Constants.Enums;

namespace Shared.Persistense.Repositories;

public class EntityStateRepository<TEntity, TContext> : EntityRepository<TEntity, TContext>, IEntityStateRepository<TEntity>
    where TEntity : class, IEntityProcessable
    where TContext : DbContext, IEntityStateDbContext
{
    private readonly TContext _context;
    private readonly string _tableName;

    protected EntityStateRepository(ILogger logger, TContext context) : base(logger, context)
    {
        _context = context;
        _tableName = context.Model.FindEntityType(typeof(TEntity))?.ShortName()
            ?? throw new SharedPersistenseEntityStateException(typeof(TEntity).Name, "Searching a table name", new("Table name not found"));
    }

    public async Task<Guid[]> PrepareDataAsync(IProcessingStep step, int limit, CancellationToken cToken)
    {
        var query = @$"
                UPDATE ""{_tableName}"" SET
	                  ""{nameof(IEntityProcessable.ProcessStatusId)}"" = {(int)Statuses.Processing}
	                , ""{nameof(IEntityProcessable.ProcessAttempt)}"" = CASE WHEN ""{nameof(IEntityProcessable.ProcessStepId)}"" = {step.Id} THEN ""{nameof(IEntityProcessable.ProcessAttempt)}"" + 1 ELSE ""{nameof(IEntityProcessable.ProcessAttempt)}"" END
	                , ""{nameof(IEntityProcessable.Updated)}"" = NOW()
                WHERE ""{nameof(IEntityProcessable.Id)}"" IN 
	                ( SELECT ""{nameof(IEntityProcessable.Id)}""
	                  FROM ""{_tableName}""
	                  WHERE ""{nameof(IEntityProcessable.ProcessStepId)}"" = {step.Id} AND ""{nameof(IEntityProcessable.ProcessStatusId)}"" = {(int)Statuses.Ready} 
	                  LIMIT {limit}
	                  FOR UPDATE SKIP LOCKED )
                RETURNING ""{nameof(IEntityProcessable.Id)}"";";

        var result = await _context.GuidIds.FromSqlRaw(query).ToArrayAsync(cToken);

        return result.Select(x => x.Id).ToArray();
    }
    public async Task<Guid[]> PrepareRetryDataAsync(IProcessingStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken)
    {
        var query = @$"
                UPDATE ""{_tableName}"" SET
	                  ""{nameof(IEntityProcessable.ProcessStatusId)}"" = {(int)Statuses.Processing}
	                , ""{nameof(IEntityProcessable.ProcessAttempt)}"" = CASE WHEN ""{nameof(IEntityProcessable.ProcessStepId)}"" = {step.Id} THEN ""{nameof(IEntityProcessable.ProcessAttempt)}"" + 1 ELSE ""{nameof(IEntityProcessable.ProcessAttempt)}"" END
	                , ""{nameof(IEntityProcessable.Updated)}"" = NOW()
                WHERE ""{nameof(IEntityProcessable.Id)}"" IN 
	                ( SELECT ""{nameof(IEntityProcessable.Id)}""
	                  FROM ""{_tableName}""
	                  WHERE 
                            ""{nameof(IEntityProcessable.ProcessStepId)}"" = {step.Id} 
                            AND ((""{nameof(IEntityProcessable.ProcessStatusId)}"" = {(int)Statuses.Processing} AND ""{nameof(IEntityProcessable.Updated)}"" < '{updateTime: yyyy-MM-dd HH:mm:ss}') OR (""{nameof(IEntityProcessable.ProcessStatusId)}"" = {(int)Statuses.Error}))
			                AND ""{nameof(IEntityProcessable.ProcessAttempt)}"" < {maxAttempts}
	                  LIMIT {limit}
	                  FOR UPDATE SKIP LOCKED )
                RETURNING ""{nameof(IEntityProcessable.Id)}"";";

        var result = await _context.GuidIds.FromSqlRaw(query).ToArrayAsync(cToken);

        return result.Select(x => x.Id).ToArray();
    }
    
    public Task<TEntity[]> GetDataAsync(IProcessingStep step, IEnumerable<Guid> ids, CancellationToken cToken) =>
        _context.Set<TEntity>().Where(x => x.ProcessStepId == step.Id && ids.Contains(x.Id)).ToArrayAsync(cToken);
    
    public Task SaveResultAsync(IProcessingStep? step, IEnumerable<TEntity> entities, CancellationToken cToken)
    {
        var array = entities.ToArray();

        if (step is null)
            return UpdateRangeAsync(array, cToken);

        foreach (var entity in array.Where(x => x.ProcessStatusId != (int)Statuses.Error))
            entity.ProcessStepId = step.Id;

        return UpdateRangeAsync(array, cToken);
    }
}