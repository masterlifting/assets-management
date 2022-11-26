using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.Catalogs;
using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public class ProcessableStepRepository<TContext> : SqlCatalogRepository<ProcessStep, TContext>, IProcessableStepRepository
    where TContext : DbContext
    {
        public ProcessableStepRepository(ILogger<EntityCatalog> logger, TContext context) : base(logger, context)
        {
        }
    }
}
