namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs.Models;

public sealed class BcsReportModel
{
    public string Agreement { get; init; } = null!;
    public string DateStart { get; set; } = null!;
    public string DateEnd { get; set; } = null!;

    public IEnumerable<BcsReportDividendModel>? Dividends { get; set; }
    public IEnumerable<BcsReportComissionModel>? Comissions { get; set; }
    public IEnumerable<BcsReportBalanceModel>? Balances { get; set; }
    public IEnumerable<BcsReportExchangeRateModel>? ExchangeRates { get; set; }
    public IEnumerable<BcsReportTransactionModel>? Transactions { get; set; }
    public IEnumerable<BcsReportStockMoveModel>? StockMoves { get; set; }
}