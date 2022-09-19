using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Operations;
using Shared.Infrastructure.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

public interface IExpenseRepository : IRepository<Expense>
{
}