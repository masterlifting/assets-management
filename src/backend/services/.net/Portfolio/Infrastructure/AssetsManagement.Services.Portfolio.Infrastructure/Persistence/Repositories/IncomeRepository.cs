using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Operations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public class IncomeRepository<TContext> : Repository<Income, TContext>, IIncomeRepository
    where TContext : DbContext
{
    protected IncomeRepository(ILogger<Income> logger, TContext context) : base(logger, context)
    {
    }
}