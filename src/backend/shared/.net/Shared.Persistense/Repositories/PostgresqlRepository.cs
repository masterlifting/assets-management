using Microsoft.Extensions.Logging;
using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities;
using Shared.Contracts.Models.Results;
using Shared.Persistense.Abstractions.Entities.Catalogs;
using Shared.Persistense.Contexts;
using Microsoft.EntityFrameworkCore;
using static Shared.Persistense.Constants.Enums;
using Shared.Persistense.Exceptions;
using Shared.Persistense.Abstractions.Repositories;

namespace Shared.Persistense.Repositories
{
    public class PostgresqlRepository : IPostgresqlRepository
    {
        private readonly string _initiator;
        private readonly ILogger _logger;
        private readonly PostgresqContext _context;

        public PostgresqlRepository(ILogger logger, PostgresqContext context)
        {
            _logger = logger;
            _context = context;
            var objectId = base.GetHashCode();
            _initiator = $"{nameof(PostgresqlRepository)} ({objectId})";
        }

        public virtual async Task CreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IEntity
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
        public virtual async Task CreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
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
        public virtual async Task<Result> TryCreateAsync<T>(T entity, CancellationToken? cToken = null) where T : class, IEntity
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
        public virtual async Task<Result> TryCreateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
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

        public virtual async Task UpdateAsync<T>(object[] id, T entity, CancellationToken? cToken = null) where T : class, IEntity
        {
            if (await _context.Set<T>().FindAsync(id) is null)
                throw new SharedPersistenseEntityException(_initiator, Constants.Actions.Update, new($"Entity by Id: '{id}' not found"));

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
        public virtual async Task UpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
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
        public virtual async Task<Result> TryUpdateAsync<T>(object[] id, T entity, CancellationToken? cToken = null) where T : class, IEntity
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
        public virtual async Task<Result> TryUpdateRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
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

        public virtual async Task<T> DeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IEntity
        {
            var entity = await _context.Set<T>().FindAsync(id);

            if (entity is null)
                throw new SharedPersistenseEntityException(_initiator, Constants.Actions.Update, new($"Entity by Id: '{id}' not found"));

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
        public virtual async Task DeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
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
        public virtual async Task<Result> TryDeleteRangeAsync<T>(IReadOnlyCollection<T> entities, CancellationToken? cToken = null) where T : class, IEntity
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
        public virtual async Task<ResultData<T>> TryDeleteAsync<T>(object[] id, CancellationToken? cToken = null) where T : class, IEntity
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

        public Task<T[]> GetCatalogsAsync<T>() where T : class, IEntityCatalog => _context.Set<T>().ToArrayAsync();
        public Task<Dictionary<int, T>> GetCatalogsDictionaryByIdAsync<T>() where T : class, IEntityCatalog => _context.Set<T>().ToDictionaryAsync(x => x.Id);
        public Task<Dictionary<string, T>> GetCatalogsDictionaryByNameAsync<T>() where T : class, IEntityCatalog => _context.Set<T>().ToDictionaryAsync(x => x.Name);
        public ValueTask<T?> GetCatalogByIdAsync<T>(int id) where T : class, IEntityCatalog => _context.Set<T>().FindAsync(id);
        public Task<T?> GetCatalogByNameAsync<T>(string name) where T : class, IEntityCatalog => _context.Set<T>().FirstOrDefaultAsync(x => x.Name.Equals(name));

        public async Task<Guid[]> PrepareProcessableEntityDataAsync<T>(IProcessableEntityStep step, int limit, CancellationToken cToken) where T : class, IProcessableEntity
        {
            var tableName = _context.Model.FindEntityType(typeof(T))?.ShortName()
                ?? throw new SharedPersistenseEntityStateException(typeof(T).Name, "Searching a table name", new("Table name not found"));

            var query = @$"
                UPDATE ""{tableName}"" SET
	                  ""{nameof(IProcessableEntity.ProcessStatusId)}"" = {(int)Statuses.Processing}
	                , ""{nameof(IProcessableEntity.ProcessAttempt)}"" = CASE WHEN ""{nameof(IProcessableEntity.ProcessStepId)}"" = {step.Id} THEN ""{nameof(IProcessableEntity.ProcessAttempt)}"" + 1 ELSE ""{nameof(IProcessableEntity.ProcessAttempt)}"" END
	                , ""{nameof(IProcessableEntity.Updated)}"" = NOW()
                WHERE ""{nameof(IProcessableEntity.Id)}"" IN 
	                ( SELECT ""{nameof(IProcessableEntity.Id)}""
	                  FROM ""{tableName}""
	                  WHERE ""{nameof(IProcessableEntity.ProcessStepId)}"" = {step.Id} AND ""{nameof(IProcessableEntity.ProcessStatusId)}"" = {(int)Statuses.Ready} 
	                  LIMIT {limit}
	                  FOR UPDATE SKIP LOCKED )
                RETURNING ""{nameof(IProcessableEntity.Id)}"";";

            var result = await _context.GuidIds.FromSqlRaw(query).ToArrayAsync(cToken);

            return result.Select(x => x.Id).ToArray();
        }
        public async Task<Guid[]> PrepareProcessableEntityRetryDataAsync<T>(IProcessableEntityStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) where T : class, IProcessableEntity
        {
            var tableName = _context.Model.FindEntityType(typeof(T))?.ShortName()
                ?? throw new SharedPersistenseEntityStateException(typeof(T).Name, "Searching a table name", new("Table name not found"));

            var query = @$"
                UPDATE ""{tableName}"" SET
	                  ""{nameof(IProcessableEntity.ProcessStatusId)}"" = {(int)Statuses.Processing}
	                , ""{nameof(IProcessableEntity.ProcessAttempt)}"" = CASE WHEN ""{nameof(IProcessableEntity.ProcessStepId)}"" = {step.Id} THEN ""{nameof(IProcessableEntity.ProcessAttempt)}"" + 1 ELSE ""{nameof(IProcessableEntity.ProcessAttempt)}"" END
	                , ""{nameof(IProcessableEntity.Updated)}"" = NOW()
                WHERE ""{nameof(IProcessableEntity.Id)}"" IN 
	                ( SELECT ""{nameof(IProcessableEntity.Id)}""
	                  FROM ""{tableName}""
	                  WHERE 
                            ""{nameof(IProcessableEntity.ProcessStepId)}"" = {step.Id} 
                            AND ((""{nameof(IProcessableEntity.ProcessStatusId)}"" = {(int)Statuses.Processing} AND ""{nameof(IProcessableEntity.Updated)}"" < '{updateTime: yyyy-MM-dd HH:mm:ss}') OR (""{nameof(IProcessableEntity.ProcessStatusId)}"" = {(int)Statuses.Error}))
			                AND ""{nameof(IProcessableEntity.ProcessAttempt)}"" < {maxAttempts}
	                  LIMIT {limit}
	                  FOR UPDATE SKIP LOCKED )
                RETURNING ""{nameof(IProcessableEntity.Id)}"";";

            var result = await _context.GuidIds.FromSqlRaw(query).ToArrayAsync(cToken);

            return result.Select(x => x.Id).ToArray();
        }
        public Task<T[]> GetProcessableEntityDataAsync<T>(IProcessableEntityStep step, IEnumerable<Guid> ids, CancellationToken cToken) where T : class, IProcessableEntity =>
        _context.Set<T>().Where(x => x.ProcessStepId == step.Id && ids.Contains(x.Id)).ToArrayAsync(cToken);
        public Task SaveProcessableEntityResultAsync<T>(IProcessableEntityStep? step, IEnumerable<T> entities, CancellationToken cToken) where T : class, IProcessableEntity
        {
            var array = entities.ToArray();

            if (step is null)
                return UpdateRangeAsync(array, cToken);

            foreach (var entity in array.Where(x => x.ProcessStatusId != (int)Statuses.Error))
                entity.ProcessStepId = step.Id;

            return UpdateRangeAsync(array, cToken);
        }
    }
}
