using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Exceptions;

using static Shared.Persistense.Constants.Enums;

namespace Shared.Persistense.Repositories;

public class EntityStateRepository<TEntity, TContext> : Repository<TEntity, TContext>, IEntityStateRepository<TEntity>
    where TEntity : class, IEntityState
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

    public override Task CreateAsync(TEntity entity, CancellationToken? ctToken = null)
    {
        entity.StateId = (int)States.Draft;

        return base.CreateAsync(entity, ctToken);
    }

    public override Task CreateRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken? cToken = null)
    {
        foreach (var entity in entities)
            entity.StateId = (int)States.Draft;

        return base.CreateRangeAsync(entities, cToken);
    }

    public async Task<string[]> PrepareDataAsync(IEntityStepType step, int limit, CancellationToken cToken)
    {
        var query = @$"
                UPDATE ""{_tableName}"" SET
	                  ""{nameof(IEntityState.StateId)}"" = {(int)States.Processing}
	                , ""{nameof(IEntityState.Attempt)}"" = CASE WHEN ""{nameof(IEntityState.StepId)}"" = {step.Id} THEN ""{nameof(IEntityState.Attempt)}"" + 1 ELSE ""{nameof(IEntityState.Attempt)}"" END
	                , ""{nameof(IEntityState.UpdateTime)}"" = NOW()
                WHERE ""{nameof(IEntityState.Id)}"" IN 
	                ( SELECT ""{nameof(IEntityState.Id)}""
	                  FROM ""{_tableName}""
	                  WHERE ""{nameof(IEntityState.StepId)}"" = {step.Id} AND ""{nameof(IEntityState.StateId)}"" = {(int)States.Ready} 
	                  LIMIT {limit}
	                  FOR UPDATE SKIP LOCKED )
                RETURNING ""{nameof(IEntityState.Id)}"";";

        var result = await _context.StringIds.FromSqlRaw(query).ToArrayAsync(cToken);

        return result.Select(x => x.Id).ToArray();
    }
    public async Task<string[]> PrepareRetryDataAsync(IEntityStepType step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken)
    {
        var query = @$"
                UPDATE ""{_tableName}"" SET
	                  ""{nameof(IEntityState.StateId)}"" = {(int)States.Processing}
	                , ""{nameof(IEntityState.Attempt)}"" = CASE WHEN ""{nameof(IEntityState.StepId)}"" = {step.Id} THEN ""{nameof(IEntityState.Attempt)}"" + 1 ELSE ""{nameof(IEntityState.Attempt)}"" END
	                , ""{nameof(IEntityState.UpdateTime)}"" = NOW()
                WHERE ""{nameof(IEntityState.Id)}"" IN 
	                ( SELECT ""{nameof(IEntityState.Id)}""
	                  FROM ""{_tableName}""
	                  WHERE 
                            ""{nameof(IEntityState.StepId)}"" = {step.Id} 
                            AND ((""{nameof(IEntityState.StateId)}"" = {(int)States.Processing} AND ""{nameof(IEntityState.UpdateTime)}"" < '{updateTime : yyyy-MM-dd HH:mm:ss}') OR (""{nameof(IEntityState.StateId)}"" = {(int)States.Error}))
			                AND ""{nameof(IEntityState.Attempt)}"" < {maxAttempts}
	                  LIMIT {limit}
	                  FOR UPDATE SKIP LOCKED )
                RETURNING ""{nameof(IEntityState.Id)}"";";

        var result = await _context.StringIds.FromSqlRaw(query).ToArrayAsync(cToken);
        
        return result.Select(x => x.Id).ToArray();
    }
    public Task<TEntity[]> GetDataAsync(IEntityStepType step, IEnumerable<string> ids, CancellationToken cToken) =>
        _context.Set<TEntity>().Where(x => x.StepId == step.Id && ids.Contains(x.Id)).ToArrayAsync(cToken);
    public Task SaveResultAsync(IEntityStepType? step, IEnumerable<TEntity> entities, CancellationToken cToken)
    {
        var array = entities.ToArray();

        if (step is null)
            return UpdateRangeAsync(array, cToken);

        foreach (var entity in array.Where(x => x.StateId != (int)States.Error))
            entity.StepId = step.Id;

        return UpdateRangeAsync(array, cToken);
    }
}