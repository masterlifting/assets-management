namespace AM.Services.Portfolio.Core.Services.BcsServices.Models;

public sealed record BcsReportDealModel
{
    public BcsReportEventModel IncomeEvent { get; init; } = null!;
    public BcsReportEventModel ExpenseEvent { get; init; } = null!;
}