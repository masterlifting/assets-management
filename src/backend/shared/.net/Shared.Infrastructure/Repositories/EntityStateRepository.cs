using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Contracts.Abstractions.Domains.Entities;
using Shared.Core.Background.EntityState;

namespace Shared.Infrastructure.Repositories
{
    public class EntityStateRepository<T> : DbContext where T : class, IEntityState
    {
        private readonly ILogger _logger;
        private readonly DbContext _context;
        private readonly EntityStateBackgroundService _backgroundService;
        private readonly DbSet<T> _dbSet;

        public EntityStateRepository(ILogger logger, DbContext context, EntityStateBackgroundService backgroundService)
        {
            _logger = logger;
            _context = context;
            _backgroundService = backgroundService;

            _dbSet = _context.Set<T>();
            _context.SavedChanges += RunEntityStateBackgroundService;
        }

        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public override ValueTask DisposeAsync()
        {
            _context.SavedChanges -= RunEntityStateBackgroundService;
            return base.DisposeAsync();
        }

        private async void RunEntityStateBackgroundService(object? sender, SavedChangesEventArgs args)
        {
            const int delay = 30000;//ms
            const int delayOneHandle = 500;//ms
            
            var period = args.EntitiesSavedCount * delayOneHandle;
            period = period < delay ? delay : period;
            
            var limit = args.EntitiesSavedCount / (period / delay +1);
        }
    }
}
