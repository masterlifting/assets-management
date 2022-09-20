using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Exceptions;

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
        _tableName = context.Model.FindEntityType(typeof(TEntity))?.ShortName() ?? throw new SharedPersistenseEntityStateException("", "", "Не удалось определить название таблицы");
    }

    public async Task<string[]> PrepareDataAsync(IEntityStepCatalog step, int limit, CancellationToken cToken)
    {
        var query = @$"
                DECLARE @StepId INT = {step}, @Limit INT = {limit}
                DECLARE @UpdatedIds TABLE (Id BIGINT)
                UPDATE TOP (@Limit) {_tableName} SET
	                StateId = 2, 
	                Attempt = Attempt+1,
	                UpdateTime = GETDATE()
	                OUTPUT INSERTED.Id INTO @UpdatedIds
		        WHERE StepId = @StepId AND StateId = 1 
			    SELECT Id FROM @UpdatedIds";

        var result = _context.StringIds.FromSqlRaw(query);

        return await result.Select(x => x.Id).ToArrayAsync(cToken);
    }
    public async Task<string[]> PrepareRetryDataAsync(IEntityStepCatalog step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken)
    {
        var query = @$"
                DECLARE @StepId INT = {step}, @Limit INT = {limit}, @UpdateTime DATETIME2 = {updateTime}, @MaxAttempts INT = {maxAttempts}
                DECLARE @UpdatedIds TABLE (Id BIGINT)
                UPDATE TOP (@Limit) {_tableName} SET
	                StateId = 2, 
	                Attempt = Attempt+1,
	                UpdateTime = GETDATE()
	                OUTPUT INSERTED.Id INTO @UpdatedIds
                WHERE 
			        StepId = @StepId 
			        AND ((StateId = 2 AND UpdateTime < @UpdateTime) OR (StateId = -1))
			        AND Attempt < @MaxAttempts
			    SELECT Id FROM @UpdatedIds";

        var result = _context.StringIds.FromSqlRaw(query);

        return await result.Select(x => x.Id).ToArrayAsync(cToken);
    }
    public Task<TEntity[]> GetDataAsync(IEntityStepCatalog step, IEnumerable<string> ids, CancellationToken cToken) =>
        _context.Set<TEntity>().Where(x => x.StepId == step.Id && ids.Contains(x.Id)).ToArrayAsync(cToken);
    public Task SaveResultAsync(IEntityStepCatalog? step, IEnumerable<TEntity> entities, CancellationToken cToken)
    {
        var array = entities.ToArray();

        if (step is null)
            return UpdateRangeAsync(array, cToken);

        foreach (var entity in array)
            entity.StepId = step.Id;

        return UpdateRangeAsync(array, cToken);
    }
}