using AM.Services.Portfolio.Core.Abstractions.Excel;
using AM.Services.Portfolio.Core.Domain.Persistense.Collections.BcsReport;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.BcsServices.Implementations.Helpers;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.BcsServices.Implementations.v1;

public sealed class BcsReportDataToJsonService : IBcsReportDataToJsonService
{
    private readonly string _initiator = nameof(BcsReportDataToJsonService);

    private readonly List<BcsReportEventModel> _events;
    private readonly List<BcsReportDealModel> _deals;


    private readonly IPortfolioExcelService _excelService;
    private int _rowId;

    private readonly Dictionary<string, Action<IPortfolioExcelDocument, string, Currencies?>> _reportPatterns;

    public BcsReportDataToJsonService(IPortfolioExcelService excelService)
    {
        _excelService = excelService;

        _events = Enumerable.Empty<BcsReportEventModel>().ToList();
        _deals = Enumerable.Empty<BcsReportDealModel>().ToList();

        _reportPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Приход ДС", ParseBalance },
        { "Вывод ДС", ParseBalance },
        { "Возмещение дивидендов по сделке", ParseBalance },
        { "Проценты по займам \"овернайт\"", ParseBalance },
        { "Проценты по займам \"овернайт ЦБ\"", ParseBalance },

        { "Дивиденды", ParseDividend },

        { "ISIN:", ParseTransactions },

        { "Сопряж. валюта:", ParseExchangeRate },

        { "Доп. выпуск", ParseStockShare },
        { "Сплит акций", ParseStockSplit },

        { "Урегулирование сделок", ParseComission },
        { "Вознаграждение компании", ParseComission },
        { "Вознаграждение за обслуживание счета депо", ParseComission },
        { "Хранение ЦБ", ParseComission },
        { "НДФЛ", ParseComission },
        { "Вознаграждение компании (СВОП)", ParseComission },
        { "Комиссия за займы \"овернайт ЦБ\"", ParseComission },
        { "Вознаграждение компании (репо)", ParseComission },
        { "Комиссия Биржевой гуру", ParseComission },
        { "Оплата за вывод денежных средств", ParseComission },
        { "Распределение (4*)", ParseComission }
    };
    }

    public BcsReportModel GetReportModel(byte[] payload)
    {
        var excel = _excelService.GetExcelDocument(payload);

        _deals.EnsureCapacity(excel.RowsCount / 2);
        _events.EnsureCapacity(excel.RowsCount / 2);

        var model = new BcsReportModel
        {
            Agreement = GetReportAgreement(excel, _rowId)
        };

        var (dateStart, dateEnd) = GetReportPeriod(excel, _rowId);

        model.DateStart = dateStart;
        model.DateEnd = dateEnd;

        var fileStructure = GetFileStructure(excel, 0);

        string? patternKey;

        var firstBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points.FirstBlock, StringComparison.OrdinalIgnoreCase) > -1);
        if (firstBlock is not null)
        {
            _rowId = fileStructure[firstBlock];

            var border = fileStructure.Skip(1).First().Key;

            while (!excel.TryGetCellValue(++_rowId, 1, border, out patternKey))
                if (patternKey is not null)
                    switch (patternKey)
                    {
                        case "USD":
                            GetAction(patternKey, Currencies.Usd);
                            break;
                        case "Рубль":
                            GetAction(patternKey, Currencies.Rub);
                            break;
                    }

            void GetAction(string value, Currencies currency)
            {
                while (!excel.TryGetCellValue(++_rowId, 1, new[] { $"Итого по валюте {value}:", border }, out _))
                {
                    patternKey = excel.GetCellValue(_rowId, 2);

                    if (patternKey is not null && _reportPatterns.ContainsKey(patternKey))
                        _reportPatterns[patternKey](excel, patternKey, currency);
                }
            }
        }

        var comissionsBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points.ComissionsBlock, StringComparison.OrdinalIgnoreCase) > -1);
        if (comissionsBlock is not null)
        {
            _rowId = fileStructure[comissionsBlock] + 3;

            while (!excel.TryGetCellValue(++_rowId, 1, "Итого по валюте Рубль:", out patternKey))
                if (patternKey is not null && !_reportPatterns.ContainsKey(patternKey))
                    throw new PortfolioCoreException(_initiator, $"Parsing: {comissionsBlock}", new($"Comission: '{patternKey}' was not recognezed"));
        }

        var dealsBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points.DealsBlock, StringComparison.OrdinalIgnoreCase) > -1);
        if (dealsBlock is not null)
        {
            _rowId = fileStructure[dealsBlock];

            var borders = fileStructure.Keys
                .Where(x =>
                    BcsReportFileStructure.Points.UnfinishedDealsBlock.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1
                    || BcsReportFileStructure.Points.AssetsBlock.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1
                    || BcsReportFileStructure.Points.LastBlock.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1)
                .ToArray();

            while (!excel.TryGetCellValue(++_rowId, 1, borders, out _))
            {
                patternKey = excel.GetCellValue(_rowId, 6);

                if (patternKey is not null && _reportPatterns.ContainsKey(patternKey))
                    _reportPatterns[patternKey](excel, patternKey, null);
            }
        }

        var lastBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points.LastBlock, StringComparison.OrdinalIgnoreCase) > -1);
        if (lastBlock is not null)
        {
            _rowId = fileStructure[lastBlock];

            while (!excel.TryGetCellValue(++_rowId, 1, "Дата составления отчета:", out _))
            {
                patternKey = excel.GetCellValue(_rowId, 12);

                if (patternKey is not null
                    && patternKey.Length > 5
                    && !int.TryParse(patternKey[0..3], out _)
                    && !BcsReportFileStructure.LastBlockExceptedWords.Any(x => patternKey.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                    if (_reportPatterns.Keys.Any(x => patternKey.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                    {
                        var _patternKey = string.Join(' ', patternKey.Split(' ')[0..2]);
                        var value = excel.GetCellValue(_rowId, 1)!;

                        _reportPatterns[_patternKey](excel, value, null);
                    }
                    else
                        throw new PortfolioCoreException(_initiator, $"Parsing row number: {_rowId + 1}", new($"Action: '{patternKey}' was not recognezed"));
            }
        }

        model.Events = _events;
        model.Deals = _deals;

        return model;
    }

    #region Events
    private void ParseBalance(IPortfolioExcelDocument excel, string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new PortfolioCoreException(_initiator, nameof(ParseBalance), new("Currency not found"));

        var currencyValue = currency.Value.ToString();

        var exchange = GetExchangeName(excel, nameof(ParseBalance), _rowId);

        if (!BcsReportFileStructure.BalanceEvents.ContainsKey(value))
            throw new PortfolioCoreException(_initiator, nameof(ParseBalance), new($"Event type '{value}' not recognized"));

        var eventCost = excel.GetCellValue(_rowId, BcsReportFileStructure.BalanceEvents[value].columnNo);

        if (eventCost is null)
            throw new PortfolioCoreException(_initiator, nameof(ParseBalance), new("Balance sum not found"));

        if (!decimal.TryParse(eventCost, out var eventCostDecimalValue))
            throw new PortfolioCoreException(_initiator, nameof(ParseBalance), new($"Balance sum '{eventCost}' wasn't recognized"));

        var date = excel.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, nameof(ParseBalance), new("Date not found"));

        _events.Add(new()
        {
            Asset = currencyValue,
            Value = eventCostDecimalValue,
            Date = date,
            EventType = BcsReportFileStructure.BalanceEvents[value].eventType,
            Exchange = exchange
        });
    }
    private void ParseDividend(IPortfolioExcelDocument excel, string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new PortfolioCoreException(_initiator, nameof(ParseDividend), new("Currency not found"));

        var currencyValue = currency.Value.ToString();

        var exchange = GetExchangeName(excel, nameof(ParseDividend), _rowId);

        var info = excel.GetCellValue(_rowId, 14);

        if (info is null)
            throw new PortfolioCoreException(_initiator, nameof(ParseDividend), new("Description not found"));

        var date = excel.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, nameof(ParseDividend), new("Date not found"));

        var sum = excel.GetCellValue(_rowId, 6);

        if (sum is null)
            throw new PortfolioCoreException(_initiator, nameof(ParseDividend), new("Dividend sum not found"));

        if (!decimal.TryParse(sum, out var sumDecimalValue))
            throw new PortfolioCoreException(_initiator, nameof(ParseDividend), new($"Dividend sum '{sum}' wasn't recognized"));


        _events.Add(new()
        {
            Asset = currencyValue,
            Value = sumDecimalValue,
            Date = date,
            EventType = nameof(EventTypes.Dividend),
            Exchange = exchange,
            Info = info
        });

        var taxPosition = info.IndexOf("налог", StringComparison.OrdinalIgnoreCase);

        if (taxPosition <= -1)
            return;

        var taxSum = info[taxPosition..].Split(' ')[1];

        taxSum = taxSum.IndexOf('$') > -1 ? taxSum[1..] : taxSum;

        if (!decimal.TryParse(taxSum, out var taxSumDecimalValue))
            throw new PortfolioCoreException(_initiator, nameof(ParseDividend), new($"Dividend tax sum '{taxSum}' wasn't recognized"));

        _events.Add(new()
        {
            Asset = currencyValue,
            Value = taxSumDecimalValue,
            Date = date,
            EventType = nameof(EventTypes.TaxIncome),
            Exchange = exchange,
            Info = info
        });
    }
    private void ParseComission(IPortfolioExcelDocument excel, string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new PortfolioCoreException(_initiator, nameof(ParseComission), new("Currency not found"));
        var currencyName = currency.Value.ToString();

        var sum = excel.GetCellValue(_rowId, 7);

        if (sum is null)
            throw new PortfolioCoreException(_initiator, nameof(ParseComission), new("Comission sum not found"));

        if (!decimal.TryParse(sum, out var sumDecimalValue))
            throw new PortfolioCoreException(_initiator, nameof(ParseComission), new($"Comission sum '{sum}' wasn't recognized"));

        var date = excel.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, nameof(ParseComission), new("Date not found"));

        _events.Add(new()
        {
            Asset = currencyName,
            Value = sumDecimalValue,
            Date = date,
            EventType = BcsReportFileStructure.ComissionEvents[value],
            Exchange = GetExchangeName(excel, nameof(ParseComission), _rowId)
        });
    }
    private void ParseStockShare(IPortfolioExcelDocument excel, string value, Currencies? currency)
    {
        var date = excel.GetCellValue(_rowId, 4);

        if (date is null)
            throw new PortfolioCoreException(_initiator, nameof(ParseStockShare), new("Date not found"));

        var sharedValue = excel.GetCellValue(_rowId, 7);

        if (sharedValue is null)
            throw new PortfolioCoreException(_initiator, nameof(ParseStockShare), new("Stock shared value not found"));

        if (!decimal.TryParse(sharedValue, out var sharedDecimalValue))
            throw new PortfolioCoreException(_initiator, nameof(ParseComission), new($"Stock shared value '{sharedValue}' wasn't recognized"));


        _events.Add(new()
        {
            Asset = value.Trim(),
            Value = sharedDecimalValue,
            Date = date,
            EventType = nameof(EventTypes.Donation),
            Exchange = GetExchangeName(excel, nameof(ParseStockShare), _rowId)
        });
    }
    private void ParseStockSplit(IPortfolioExcelDocument excel, string value, Currencies? currency)
    {
        var date = excel.GetCellValue(_rowId, 4);

        if (date is null)
            throw new PortfolioCoreException(_initiator, nameof(ParseStockSplit), new("Date not found"));

        var valueBefore = excel.GetCellValue(_rowId, 6);
        if (valueBefore is null || !int.TryParse(valueBefore, out var _valueBefore))
            throw new PortfolioCoreException(_initiator, nameof(ParseStockSplit), new("Stock split value before was not recognized"));

        var valueAfter = excel.GetCellValue(_rowId, 7);
        if (valueAfter is null || !int.TryParse(valueAfter, out var _valueAfter))
            throw new PortfolioCoreException(_initiator, nameof(ParseStockSplit), new("Stock split value after was not recognized"));

        var splitValue = _valueAfter / _valueBefore;

        _events.Add(new()
        {
            Asset = value.Trim(),
            Value = splitValue,
            Date = date,
            EventType = nameof(EventTypes.Splitting),
            Exchange = GetExchangeName(excel, nameof(ParseStockSplit), _rowId)
        });
    }
    #endregion

    #region Deals
    private void ParseExchangeRate(IPortfolioExcelDocument excel, string value, Currencies? currency = null)
    {
        var currencyCode = excel.GetCellValue(_rowId, 1);

        if (currencyCode is null || !BcsReportFileStructure.ExchangeCurrencies.ContainsKey(currencyCode))
            throw new PortfolioCoreException(_initiator, nameof(ParseExchangeRate), new("Currency not recognized"));

        var incomeCurrency = BcsReportFileStructure.ExchangeCurrencies[currencyCode].Income;
        var expenseCurrency = BcsReportFileStructure.ExchangeCurrencies[currencyCode].Expense;

        while (!excel.TryGetCellValue(++_rowId, 1, $"Итого по {currencyCode}:", out _))
        {
            var incomeValue = excel.GetCellValue(_rowId, 5);
            if (incomeValue is not null)
            {
                if (!decimal.TryParse(incomeValue, out var incomeDecimalValue))
                    throw new PortfolioCoreException(_initiator, nameof(ParseExchangeRate), new($"Value of income '{incomeValue}' wasn't recognized"));

                var exchange = GetExchangeName(excel, nameof(ParseExchangeRate), _rowId);

                var date = excel.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, nameof(ParseExchangeRate), new("Date not found"));

                var decreasingValue = excel.GetCellValue(_rowId, 4);

                if (decreasingValue is null)
                    throw new PortfolioCoreException(_initiator, nameof(ParseExchangeRate), new("Value of expense not found"));

                if (!decimal.TryParse(decreasingValue, out var decreasingDecimalValue))
                    throw new PortfolioCoreException(_initiator, nameof(ParseExchangeRate), new($"Value of expense '{decreasingValue}' wasn't recognized"));

                _deals.Add(new()
                {
                    IncomeEvent = new()
                    {
                        Asset = incomeCurrency,
                        Value = incomeDecimalValue,
                        Date = date,
                        Exchange = exchange
                    },
                    ExpenseEvent = new()
                    {
                        Asset = expenseCurrency,
                        Value = decreasingDecimalValue * incomeDecimalValue,
                        Date = date,
                        Exchange = exchange
                    }
                });

                continue;
            }

            var expenseValue = excel.GetCellValue(_rowId, 8);
            if (expenseValue is not null)
            {
                if (!decimal.TryParse(expenseValue, out var expenseDecimalValue))
                    throw new PortfolioCoreException(_initiator, nameof(ParseExchangeRate), new($"Value of expense '{expenseValue}' wasn't recognized"));

                var exchange = GetExchangeName(excel, nameof(ParseExchangeRate), _rowId);

                var date = excel.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, nameof(ParseExchangeRate), new("Date not found"));

                var increasingValue = excel.GetCellValue(_rowId, 7);
                if (increasingValue is null)
                    throw new PortfolioCoreException(_initiator, nameof(ParseExchangeRate), new("Value of income not found"));

                if (!decimal.TryParse(increasingValue, out var increasingDecimalValue))
                    throw new PortfolioCoreException(_initiator, nameof(ParseExchangeRate), new($"Value of income '{increasingValue}' wasn't recognized"));

                _deals.Add(new()
                {
                    ExpenseEvent = new()
                    {
                        Asset = expenseCurrency,
                        Value = expenseDecimalValue,
                        Date = date,
                        Exchange = exchange
                    },
                    IncomeEvent = new()
                    {
                        Asset = incomeCurrency,
                        Value = increasingDecimalValue * expenseDecimalValue,
                        Date = date,
                        Exchange = exchange
                    }
                });
            }
        }
    }
    private void ParseTransactions(IPortfolioExcelDocument excel, string value, Currencies? currency = null)
    {
        var isin = excel.GetCellValue(_rowId, 7);

        if (isin is null)
            throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new("ISIN not recognized"));

        var name = excel.GetCellValue(_rowId, 1);

        while (!excel.TryGetCellValue(++_rowId, 1, $"Итого по {name}:", out _))
        {
            var incomeValue = excel.GetCellValue(_rowId, 4);
            if (incomeValue is not null)
            {
                if (!decimal.TryParse(incomeValue, out var incomeDecimalValue))
                    throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new($"Value of income '{incomeValue}' wasn't recognized"));

                var exchange = GetExchangeName(excel, nameof(ParseTransactions), _rowId);

                var date = excel.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new("Date not found"));

                var currencyCode = excel.GetCellValue(_rowId, 10);
                if (currencyCode is null)
                    throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new("Currency not found"));
                var currencyName = currencyCode switch
                {
                    "USD" => Currencies.Usd.ToString(),
                    "Рубль" => Currencies.Rub.ToString(),
                    _ => throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new("Currency not recognized"))
                };

                var decreasingValue = excel.GetCellValue(_rowId, 5);

                if (decreasingValue is null)
                    throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new("Value of expense not found"));

                if (!decimal.TryParse(decreasingValue, out var decreasingDecimalValue))
                    throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new($"Value of expense '{decreasingValue}' wasn't recognized"));

                _deals.Add(new()
                {
                    IncomeEvent = new()
                    {
                        Asset = isin,
                        Value = incomeDecimalValue,
                        Date = date,
                        Exchange = exchange
                    }
                });
                _deals.Add(new()
                {
                    ExpenseEvent = new()
                    {
                        Asset = currencyName,
                        Value = decreasingDecimalValue * incomeDecimalValue,
                        Date = date,
                        Exchange = exchange
                    }
                });

                continue;
            }

            var expenseValue = excel.GetCellValue(_rowId, 7);
            if (expenseValue is not null)
            {
                if (!decimal.TryParse(expenseValue, out var expenseDecimalValue))
                    throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new($"Value of expense '{expenseValue}' wasn't recognized"));

                var exchange = GetExchangeName(excel, nameof(ParseTransactions), _rowId);

                var date = excel.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new("Date not found"));

                var currencyCode = excel.GetCellValue(_rowId, 10);
                if (currencyCode is null)
                    throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new("Currency not found"));
                var currencyName = currencyCode switch
                {
                    "USD" => Currencies.Usd.ToString(),
                    "Рубль" => Currencies.Rub.ToString(),
                    _ => throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new("Cirrency not recognized"))
                };

                var increasingValue = excel.GetCellValue(_rowId, 8);

                if (increasingValue is null)
                    throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new("Value of income not found"));

                if (!decimal.TryParse(increasingValue, out var increasingDecimalValue))
                    throw new PortfolioCoreException(_initiator, nameof(ParseTransactions), new($"Value of income '{increasingValue}' wasn't recognized"));

                _deals.Add(new()
                {
                    ExpenseEvent = new()
                    {
                        Asset = isin,
                        Value = expenseDecimalValue,
                        Date = date,
                        Exchange = exchange
                    },
                    IncomeEvent = new()
                    {
                        Asset = currencyName,
                        Value = increasingDecimalValue * expenseDecimalValue,
                        Date = date,
                        Exchange = exchange
                    }
                });
            }
        }
    }
    #endregion

    private Dictionary<string, int> GetFileStructure(IPortfolioExcelDocument excel, int rowId)
    {
        var fileStructurePoints = new string[]
        {
    BcsReportFileStructure.Points.FirstBlock
    , BcsReportFileStructure.Points.LoansBlock
    , BcsReportFileStructure.Points.ComissionsBlock
    , BcsReportFileStructure.Points.DealsBlock
    , BcsReportFileStructure.Points.UnfinishedDealsBlock
    , BcsReportFileStructure.Points.AssetsBlock
    , BcsReportFileStructure.Points.LastBlock
        };

        var result = new Dictionary<string, int>(fileStructurePoints.Length);

        while (!excel.TryGetCellValue(++rowId, 1, "Дата составления отчета:", out var cellValue))
            if (cellValue is not null && fileStructurePoints.Any(x => cellValue.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                result.Add(cellValue, rowId);

        return !result.Any()
            ? throw new PortfolioCoreException(_initiator, nameof(GetFileStructure), new("The structure of the file was not recognized"))
            : result;
    }
    private (string DateStart, string DateEnd) GetReportPeriod(IPortfolioExcelDocument excel, int rowId)
    {
        string? period = null;

        while (!excel.TryGetCellValue(rowId++, 1, "Период:", out _))
            period = excel.GetCellValue(rowId, 5);

        if (string.IsNullOrWhiteSpace(period))
            throw new PortfolioCoreException(_initiator, nameof(GetReportPeriod), new("The period was not found"));

        try
        {
            var periods = period.Split('\u0020');
            return (periods[1], periods[3]);
        }
        catch (Exception exception)
        {
            throw new PortfolioCoreException(_initiator, nameof(GetReportPeriod), new(exception));
        }
    }
    private string GetReportAgreement(IPortfolioExcelDocument excel, int rowId)
    {
        string? agreement = null;

        while (!excel.TryGetCellValue(rowId++, 1, "Генеральное соглашение:", out _))
            agreement = excel.GetCellValue(rowId, 5);

        return string.IsNullOrWhiteSpace(agreement)
            ? throw new PortfolioCoreException(_initiator, nameof(GetReportAgreement), new("The agreement was not found"))
            : agreement;
    }
    private string GetExchangeName(IPortfolioExcelDocument excel, string actionName, int rowId)
    {
        for (var columnNo = 10; columnNo < 20; columnNo++)
        {
            var exchange = excel.GetCellValue(rowId, columnNo);

            if (!string.IsNullOrEmpty(exchange)
                && exchange != "0"
                && !int.TryParse(exchange[0].ToString(), out _)
                && BcsReportFileStructure.ExchangeTypes.ContainsKey(exchange))
                return BcsReportFileStructure.ExchangeTypes[exchange].ToString().ToUpper();
        }

        throw new PortfolioCoreException(_initiator, actionName, new($"The exchange name was not recognized in the row number: {rowId + 1}"));
    }
}