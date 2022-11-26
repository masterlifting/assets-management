using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class ExpenseRepository<TContext> : SqlEntityRepository<Expense, TContext>, IExpenseRepository
    where TContext : DbContext
{
    public ExpenseRepository(ILogger<Expense> logger, TContext context) : base(logger, context)
    {
    }
}