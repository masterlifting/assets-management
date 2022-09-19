using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Operations;

using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;

public interface IExpenseRepository : IRepository<Expense>
{
}