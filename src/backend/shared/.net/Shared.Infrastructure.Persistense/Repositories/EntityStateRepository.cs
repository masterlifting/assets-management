using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Infrastructure.Persistense.Abstractions.Entities.State;
using Shared.Infrastructure.Persistense.Abstractions.Entities.State.Handle;
using Shared.Infrastructure.Persistense.Abstractions.Repositories;
using Shared.Infrastructure.Persistense.Exceptions;

namespace Shared.Infrastructure.Persistense.Repositories;

public class EntityStateRepository<TEntity, TContext> : Repository<TEntity, TContext>, IEntityStateRepository<TEntity>
    where TEntity : class, IEntityState
    where TContext : DbContext, IEntityStateDbContext
{
    private readonly TContext _context;
    private readonly string _tableName;

    protected EntityStateRepository(ILogger logger, TContext context) : base(logger, context)
    {
        _context = context;
        _tableName = context.Model.FindEntityType(typeof(TEntity))?.ShortName() ?? throw new PersistenseEntityStateException("", "", "Не удалось определить название таблицы");
    }

    public async Task<string[]> PrepareDataAsync(int stepId, int limit, CancellationToken cToken)
    {
        var query = @$"
                DECLARE @StepId INT = {stepId}, @Limit INT = {limit}
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
    public async Task<string[]> PrepareRetryDataAsync(int stepId, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken)
    {
        var query = @$"
                DECLARE @StepId INT = {stepId}, @Limit INT = {limit}, @UpdateTime DATETIME2 = {updateTime}, @MaxAttempts INT = {maxAttempts}
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
    public Task<TEntity[]> GetDataAsync(int stepId, IEnumerable<string> ids, CancellationToken cToken) =>
        _context.Set<TEntity>().Where(x => x.StepId == stepId && ids.Contains(x.Id)).ToArrayAsync(cToken);
    public Task SaveResultAsync(int? stepId, IEnumerable<TEntity> entities, CancellationToken cToken)
    {
        var array = entities.ToArray();

        if (!stepId.HasValue)
            return UpdateRangeAsync(array, cToken);

        foreach (var entity in array)
            entity.StepId = stepId.Value;

        return UpdateRangeAsync(array, cToken);
    }
}