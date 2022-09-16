using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Operations;
using AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Infrastructure.Persistense.Repositories.Implementation;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public class ExpenseRepository<TContext> : Repository<Expense, TContext>, IExpenseRepository
    where TContext : DbContext
{
    protected ExpenseRepository(ILogger<Expense> logger, TContext context) : base(logger, context)
    {
    }
}