using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public sealed class IncomeRepository<TContext> : Repository<Income, TContext>, IIncomeRepository
        where TContext : DbContext
    {
        public IncomeRepository(ILogger<Income> logger, TContext context) : base(logger, context)
        {
        }
    }
}