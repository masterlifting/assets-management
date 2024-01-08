using AM.Portfolio.Core.Abstractions.Documents.Excel;
using AM.Portfolio.Core.Exceptions;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Services.BcsServices.Implementations.Helpers;
using AM.Portfolio.Core.Services.DataHeapServices.Bcs.Helpers;

using static AM.Portfolio.Core.Constants.Enums;

namespace AM.Portfolio.Core.Services.DataHeapServices.Bcs.Transactions.Parsers;

internal sealed class BcsEventParser
{
    public ICollection<BcsEvent> Events { get; }

    private readonly List<BcsEvent> _events;
    private readonly IPortfolioExcelDocument _document;
    private readonly Dictionary<BcsReportEvents, Action<BcsReportEvents, int, BcsAsset>> _patterns;
    internal BcsEventParser(IPortfolioExcelDocument document)
    {
        _events = new List<BcsEvent>(100);
        Events = _events;

        _document = document;
        _patterns = new()
        {
            { BcsReportEvents.Income, ParseBalance },
            { BcsReportEvents.Expense, ParseBalance },

            { BcsReportEvents.IncomeDividend, ParseDividend },
            { BcsReportEvents.IncomePercentage, ParseProfit },

            { BcsReportEvents.Sharing, ParseStockShare },
            { BcsReportEvents.Splitting, ParseStockSplit },

            { BcsReportEvents.CommissionBroker, ParseCommission },
            { BcsReportEvents.CommissionDepositary, ParseCommission },
            { BcsReportEvents.TaxZone, ParseCommission },
        };
    }

    public void Parse(string patternKey, int rowId, BcsAsset asset)
    {
        if (BcsFileStructure.ReportEvents.TryGetValue(patternKey, out var bcsEvent))
        {
            if (_patterns.TryGetValue(bcsEvent, out var handler))
                handler.Invoke(bcsEvent, rowId, asset);
            else
                throw new PortfolioCoreException($"The event '{bcsEvent}' does not have a handler.");
        }
    }

    private void ParseBalance(BcsReportEvents bcsEvent, int rowId, BcsAsset asset)
    {
        var eventValue = bcsEvent switch
        {
            BcsReportEvents.Income => _document.TryGetCell(rowId, 6, out var cellValue)
                ? BcsHelper.GetDecimal(cellValue)
                : throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the balance parsing."),
            BcsReportEvents.Expense => _document.TryGetCell(rowId, 7, out var cellValue)
                ? BcsHelper.GetDecimal(cellValue)
                : throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the balance parsing."),
            _ => throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the balance parsing.")
        };

        if (!_document.TryGetCell(rowId, 1, out var dateCellValue))
            throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the balance parsing.");

        var date = BcsHelper.GetDateTime(dateCellValue);
        var exchange = BcsHelper.GetExchange(_document, rowId);

        _events.Add(new()
        {
            Asset = asset,
            Value = eventValue,
            Date = date,
            Event = bcsEvent,
            Exchange = exchange
        });
    }
    private void ParseProfit(BcsReportEvents bcsEvent, int rowId, BcsAsset asset)
    {
        var eventValue = bcsEvent switch
        {
            BcsReportEvents.IncomeDividend or BcsReportEvents.IncomePercentage => _document.TryGetCell(rowId, 6, out var cellValue)
                ? BcsHelper.GetDecimal(cellValue)
                : throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the profit parsing."),
            _ => throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the profit parsing.")
        };

        if (!_document.TryGetCell(rowId, 1, out var dateCellValue))
            throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the profit parsing.");

        var date = BcsHelper.GetDateTime(dateCellValue);
        var exchange = BcsHelper.GetExchange(_document, rowId);

        _events.Add(new()
        {
            Asset = asset,
            Value = eventValue,
            Date = date,
            Event = bcsEvent,
            Exchange = exchange
        });
    }
    private void ParseDividend(BcsReportEvents bcsEvent, int rowId, BcsAsset asset)
    {
        if (!_document.TryGetCell(rowId, 14, out var dividendInfo))
            throw new PortfolioCoreException("Dividend description was not found.");

        if (!_document.TryGetCell(rowId, 1, out var dateCellValue))
            throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the dividend parsing.");

        var date = BcsHelper.GetDateTime(dateCellValue);

        if (!_document.TryGetCell(rowId, 6, out var eventCellValue))
            throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the dividend parsing.");

        var eventValue = BcsHelper.GetDecimal(eventCellValue);
        var exchange = BcsHelper.GetExchange(_document, rowId);

        _events.Add(new()
        {
            Asset = asset,
            Value = eventValue,
            Date = date,
            Event = bcsEvent,
            Exchange = exchange,
            Info = dividendInfo
        });

        var taxPosition = dividendInfo.IndexOf("налог", StringComparison.OrdinalIgnoreCase);

        if (taxPosition <= -1)
            return;

        var taxSumData = dividendInfo[taxPosition..].Split(' ')[1];
        var taxValue = BcsHelper.GetDecimal(taxSumData.IndexOf('$') > -1
            ? taxSumData[1..]
            : taxSumData);

        _events.Add(new()
        {
            Asset = asset,
            Value = taxValue,
            Date = date,
            Event = BcsReportEvents.TaxZone,
            Exchange = exchange,
            Info = dividendInfo
        });
    }
    private void ParseCommission(BcsReportEvents bcsEvent, int rowId, BcsAsset asset)
    {
        var eventValue = bcsEvent switch
        {
            BcsReportEvents.CommissionBroker or BcsReportEvents.CommissionDepositary or BcsReportEvents.TaxZone =>
                _document.TryGetCell(rowId, 7, out var cellValue)
                    ? BcsHelper.GetDecimal(cellValue)
                    : throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the comission parsing."),
            _ => throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the comission parsing.")
        };

        if (!_document.TryGetCell(rowId, 1, out var dateCellValue))
            throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the comission parsing.");

        var date = BcsHelper.GetDateTime(dateCellValue);
        var exchange = BcsHelper.GetExchange(_document, rowId);

        _events.Add(new()
        {
            Asset = asset,
            Value = eventValue,
            Date = date,
            Event = bcsEvent,
            Exchange = exchange
        });
    }
    private void ParseStockShare(BcsReportEvents bcsEvent, int rowId, BcsAsset asset)
    {
        if (!_document.TryGetCell(rowId, 4, out var dateCellValue))
            throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the stock share parsing.");

        if (!_document.TryGetCell(rowId, 7, out var eventValue))
            throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the stock share parsing.");

        _events.Add(new()
        {
            Asset = asset,
            Value = BcsHelper.GetDecimal(eventValue),
            Date = BcsHelper.GetDateTime(dateCellValue),
            Exchange = BcsHelper.GetExchange(_document, rowId),
            Event = bcsEvent,
        });
    }
    private void ParseStockSplit(BcsReportEvents bcsEvent, int rowId, BcsAsset asset)
    {
        if (!_document.TryGetCell(rowId, 6, out var beforeCellValue))
            throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the stock split parsing.");

        if (!_document.TryGetCell(rowId, 7, out var afterCellValue))
            throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the stock split parsing.");

        var valueBefore = BcsHelper.GetInteger(beforeCellValue);
        var valueAfter = BcsHelper.GetInteger(afterCellValue);

        var splitValue = valueAfter / valueBefore;

        if (!_document.TryGetCell(rowId, 4, out var dateCellValue))
            throw new PortfolioCoreException($"The event '{bcsEvent}' was not recognized for the stock split parsing.");

        _events.Add(new()
        {
            Asset = asset,
            Value = splitValue,
            Date = BcsHelper.GetDateTime(dateCellValue),
            Exchange = BcsHelper.GetExchange(_document, rowId),
            Event = bcsEvent
        });
    }
}
