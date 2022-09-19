using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Operations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public class ExpenseRepository<TContext> : Repository<Expense, TContext>, IExpenseRepository
    where TContext : DbContext
{
    protected ExpenseRepository(ILogger<Expense> logger, TContext context) : base(logger, context)
    {
    }
}