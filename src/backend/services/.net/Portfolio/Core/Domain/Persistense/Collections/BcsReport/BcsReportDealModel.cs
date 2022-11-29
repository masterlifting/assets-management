namespace AM.Services.Portfolio.Core.Domain.Persistense.Collections.BcsReport;

public sealed record BcsReportDealModel
{
    public BcsReportEventModel IncomeEvent { get; init; } = null!;
    public BcsReportEventModel ExpenseEvent { get; init; } = null!;
}