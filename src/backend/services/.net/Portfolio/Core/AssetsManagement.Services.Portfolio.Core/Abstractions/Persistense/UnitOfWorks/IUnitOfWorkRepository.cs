﻿using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.UnitOfWorks;

public interface IUnitOfWorkRepository
{
    IReportRepository Report { get; }
    IEventRepository Event { get; }
    IDealRepository Deal { get; }
    IAccountRepository Account { get; }
    IDerivativeRepository Derivative { get; }
    IAssetRepository Asset { get; }
    IIncomeRepository Income { get; }
    IExpenseRepository Expense { get; }
    IUserRepository User { get; }
}