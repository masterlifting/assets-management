using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Operations;
using AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public class IncomeRepository<TContext> : Repository<Income, TContext>, IIncomeRepository
    where TContext : DbContext
{
    protected IncomeRepository(ILogger<Income> logger, TContext context) : base(logger, context)
    {
    }
}