namespace AM.Services.Portfolio.Core.Domain.EntityStateModels.Report.Bcs;

public sealed record BcsDealTypeModel
{
    public BcsEventTypeModel IncomeEvent { get; init; } = null!;
    public BcsEventTypeModel ExpenseEvent { get; init; } = null!;
}