using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Microsoft.Extensions.Logging;
using Shared.Persistence.Abstractions.Contexts;
using Shared.Persistence.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public sealed class ProcessStepRepository : PostgreRepository<ProcessStep>, IProcessStepRepository
    {
        public ProcessStepRepository(ILogger<ProcessStep> logger, IPostgrePersistenceContext context) : base(logger, context)
        {
        }
    }
}
