using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs.Models;

using Shared.Data.Excel;
using Shared.Exceptions.Models;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs;

public sealed class BcsReportParser
{
    private string _initiator = nameof(BcsReportParser);

    private readonly ExcelDocument _excelDocument;
    private int _rowId;

    private const string DividendsParsing = nameof(DividendsParsing);
    private const string ComissionsParsing = nameof(ComissionsParsing);
    private const string BalanceParsing = nameof(BalanceParsing);
    private const string ExchangeRatesParsing = nameof(ExchangeRatesParsing);
    private const string TransactionsParsing = nameof(TransactionsParsing);
    private const string StockActionsParsing = nameof(StockActionsParsing);

    private readonly List<BcsReportDividendModel> _dividends;
    private readonly List<BcsReportComissionModel> _comissions;
    private readonly List<BcsReportBalanceModel> _balances;
    private readonly List<BcsReportExchangeRateModel> _exchangeRates;
    private readonly List<BcsReportTransactionModel> _transactions;
    private readonly List<BcsReportStockActionModel> _stockActions;

    private readonly Dictionary<string, Action<string, Currencies?>> _reportPatterns;

    public BcsReportParser(byte[] payload)
    {
        _excelDocument = GetExcelDocument(payload);

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

        _dividends = new List<BcsReportDividendModel>(_excelDocument.RowsCount);
        _comissions = new List<BcsReportComissionModel>(_excelDocument.RowsCount);
        _balances = new List<BcsReportBalanceModel>(_excelDocument.RowsCount);
        _exchangeRates = new List<BcsReportExchangeRateModel>(_excelDocument.RowsCount);
        _transactions = new List<BcsReportTransactionModel>(_excelDocument.RowsCount);
        _stockActions = new List<BcsReportStockActionModel>(_excelDocument.RowsCount);
    }


    public BcsReportModel GetReportModel()
    {
        var model = new BcsReportModel
        {
            Agreement = GetReportAgreement(_rowId)
        };

        var (dateStart, dateEnd) = GetReportPeriod(_rowId);

        model.DateStart = dateStart;
        model.DateEnd = dateEnd;

        var fileStructure = GetFileStructure(0);

        string? patternKey;

        var firstBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points[0], StringComparison.OrdinalIgnoreCase) > -1);
        if (firstBlock is not null)
        {
            _rowId = fileStructure[firstBlock];

            var border = fileStructure.Skip(1).First().Key;

            while (!_excelDocument.TryGetCellValue(++_rowId, 1, border, out patternKey))
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
                while (!_excelDocument.TryGetCellValue(++_rowId, 1, new[] { $"Итого по валюте {value}:", border }, out _))
                {
                    patternKey = _excelDocument.GetCellValue(_rowId, 2);

                    if (patternKey is not null && _reportPatterns.ContainsKey(patternKey))
                        _reportPatterns[patternKey](patternKey, currency);
                }
            }
        }

        var secondBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points[2], StringComparison.OrdinalIgnoreCase) > -1);
        if (secondBlock is not null)
        {
            _rowId = fileStructure[secondBlock] + 3;

            while (!_excelDocument.TryGetCellValue(++_rowId, 1, "Итого по валюте Рубль:", out patternKey))
                if (patternKey is not null && !_reportPatterns.ContainsKey(patternKey))
                    throw new PortfolioCoreException(_initiator, $"Parsing: {secondBlock}", new($"Comission: '{patternKey}' was not recognezed"));
        }

        var thirdBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points[3], StringComparison.OrdinalIgnoreCase) > -1);
        if (thirdBlock is not null)
        {
            _rowId = fileStructure[thirdBlock];

            var borders = fileStructure.Keys
                .Where(x =>
                    BcsReportFileStructure.Points[4].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1
                    || BcsReportFileStructure.Points[5].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1
                    || BcsReportFileStructure.Points[5].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1)
                .ToArray();

            while (!_excelDocument.TryGetCellValue(++_rowId, 1, borders, out _))
            {
                patternKey = _excelDocument.GetCellValue(_rowId, 6);

                if (patternKey is not null && _reportPatterns.ContainsKey(patternKey))
                    _reportPatterns[patternKey](patternKey, null);
            }
        }

        var fourthBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points[6], StringComparison.OrdinalIgnoreCase) > -1);
        if (fourthBlock is not null)
        {
            _rowId = fileStructure[fourthBlock];

            while (!_excelDocument.TryGetCellValue(++_rowId, 1, "Дата составления отчета:", out _))
            {
                patternKey = _excelDocument.GetCellValue(_rowId, 12);

                if (patternKey is not null
                    && patternKey.Length > 5
                    && !int.TryParse(patternKey[0..3], out _)
                    && !BcsReportFileStructure.FourthBlockExceptedWords.Any(x => patternKey.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                    if (_reportPatterns.Keys.Any(x => patternKey.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                    {
                        var _patternKey = string.Join(' ', patternKey.Split(' ')[0..2]);
                        var value = _excelDocument.GetCellValue(_rowId, 1)!;

                        _reportPatterns[_patternKey](value, null);
                    }
                    else
                        throw new PortfolioCoreException(_initiator, $"Parsing row number: {_rowId + 1}", new($"Action: '{patternKey}' was not recognezed"));
            }
        }

        model.Dividends = _dividends;
        model.Balances = _balances;
        model.Comissions = _comissions;
        model.ExchangeRates = _exchangeRates;
        model.Transactions = _transactions;
        model.StockMoves = _stockActions;

        return model;
    }

    private void ParseDividend(string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new PortfolioCoreException(_initiator, DividendsParsing, new("Currency not found"));

        var currencyValue = currency.Value.ToString();
        var exchange = GetExchangeName(DividendsParsing, _rowId);

        var info = _excelDocument.GetCellValue(_rowId, 14);

        if (info is null)
            throw new PortfolioCoreException(_initiator, DividendsParsing, new("Description not found"));

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, DividendsParsing, new("Date not found"));

        var sum = _excelDocument.GetCellValue(_rowId, 6);

        if (sum is null)
            throw new PortfolioCoreException(_initiator, DividendsParsing, new("Sum not found"));

        _dividends.Add(new()
        {
            Info = info,
            Date = date,
            Exchange = exchange,
            Sum = sum,
            Currency = currencyValue,
            EventType = nameof(EventTypes.Dividend)
        });

        var taxPosition = info.IndexOf("налог", StringComparison.OrdinalIgnoreCase);

        if (taxPosition <= -1)
            return;

        var taxSum = info[taxPosition..].Split(' ')[1];
        taxSum = taxSum.IndexOf('$') > -1 ? taxSum[1..] : taxSum;

        _comissions.Add(new()
        {
            Date = date,
            Exchange = exchange,
            Sum = taxSum,
            Currency = currencyValue,
            EventType = nameof(EventTypes.TaxIncome)
        });
    }
    private void ParseComission(string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new PortfolioCoreException(_initiator, ComissionsParsing, new("Currency not found"));
        var currencyName = currency.Value.ToString();

        var exchange = GetExchangeName(ComissionsParsing, _rowId);

        var sum = _excelDocument.GetCellValue(_rowId, 7);

        if (sum is null)
            throw new PortfolioCoreException(_initiator, ComissionsParsing, new("Sum not found"));

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, ComissionsParsing, new("Date not found"));
        
        _comissions.Add(new()
        {
            Date = date,
            Exchange = exchange,
            Sum = sum,
            Currency = currencyName,
            EventType = BcsReportFileStructure.ComissionEvents[value]
        });
    }
    private void ParseBalance(string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new PortfolioCoreException(_initiator, BalanceParsing, new("Currency not found"));

        var currencyValue = currency.Value.ToString();
        var exchange = GetExchangeName(BalanceParsing, _rowId);

        if (!BcsReportFileStructure.BalanceEvents.ContainsKey(value))
            throw new PortfolioCoreException(_initiator, BalanceParsing, new($"Event type '{value}' not recognized"));

        var sum = _excelDocument.GetCellValue(_rowId, BcsReportFileStructure.BalanceEvents[value].columnNo);

        if (sum is null)
            throw new PortfolioCoreException(_initiator, BalanceParsing, new("Sum not found"));

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, BalanceParsing, new("Date not found"));

        _balances.Add(new()
        {
            Date = date,
            Exchange = exchange,
            Sum = sum,
            Currency = currencyValue,
            EventType = BcsReportFileStructure.BalanceEvents[value].eventType
        });
    }
    private void ParseExchangeRate(string value, Currencies? currency = null)
    {
        var currencyCode = _excelDocument.GetCellValue(_rowId, 1);

        if (currencyCode is null || !BcsReportFileStructure.ExchangeCurrencies.ContainsKey(currencyCode))
            throw new PortfolioCoreException(_initiator, ExchangeRatesParsing, new("Currency not recognized"));

        var incomeCurrency = BcsReportFileStructure.ExchangeCurrencies[currencyCode].Income;
        var expenseCurrency = BcsReportFileStructure.ExchangeCurrencies[currencyCode].Expense;

        while (!_excelDocument.TryGetCellValue(++_rowId, 1, $"Итого по {currencyCode}:", out _))
        {
            var incomeValue = _excelDocument.GetCellValue(_rowId, 5);
            if (incomeValue is not null && decimal.TryParse(incomeValue, out _))
            {
                var exchange = GetExchangeName(ExchangeRatesParsing, _rowId);

                var date = _excelDocument.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesParsing, new("Date not found"));

                var sum = _excelDocument.GetCellValue(_rowId, 4);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesParsing, new("Sum not found"));

                _exchangeRates.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = incomeValue,
                    Sum = sum,
                    Currency = incomeCurrency,
                    EventType = nameof(EventTypes.Increase),
                });

                continue;
            }

            var expenseValue = _excelDocument.GetCellValue(_rowId, 8);
            if (expenseValue is not null && decimal.TryParse(expenseValue, out _))
            {
                var exchange = GetExchangeName(ExchangeRatesParsing, _rowId);

                var date = _excelDocument.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesParsing, new("Date not found"));

                var sum = _excelDocument.GetCellValue(_rowId, 7);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesParsing, new("Sum not found"));

                _exchangeRates.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = expenseValue,
                    Sum = sum,
                    Currency = expenseCurrency,
                    EventType = nameof(EventTypes.Decrease),
                });
            }
        }
    }
    private void ParseTransactions(string value, Currencies? currency = null)
    {
        var isin = _excelDocument.GetCellValue(_rowId, 7);

        if (isin is null)
            throw new PortfolioCoreException(_initiator, TransactionsParsing, new("ISIN not recognized"));

        var name = _excelDocument.GetCellValue(_rowId, 1);

        while (!_excelDocument.TryGetCellValue(++_rowId, 1, $"Итого по {name}:", out _))
        {
            var incomeValue = _excelDocument.GetCellValue(_rowId, 4);
            if (incomeValue is not null && decimal.TryParse(incomeValue, out _))
            {
                var exchange = GetExchangeName(TransactionsParsing, _rowId);

                var date = _excelDocument.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, TransactionsParsing, new("Date not found"));

                var currencyCode = _excelDocument.GetCellValue(_rowId, 10);
                if (currencyCode is null)
                    throw new PortfolioCoreException(_initiator, TransactionsParsing, new("Currency not found"));
                var currencyName = currencyCode switch
                {
                    "USD" => Currencies.Usd.ToString(),
                    "Рубль" => Currencies.Rub.ToString(),
                    _ => throw new PortfolioCoreException(_initiator, TransactionsParsing, new("Currency not recognized"))
                };

                var sum = _excelDocument.GetCellValue(_rowId, 5);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, TransactionsParsing, new("Sum not found"));

                _transactions.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = incomeValue,
                    Sum = sum,
                    Currency = currencyName,
                    EventType = nameof(EventTypes.Increase),
                    Info = isin
                });

                continue;
            }

            var expenseValue = _excelDocument.GetCellValue(_rowId, 7);
            if (expenseValue is not null && decimal.TryParse(expenseValue, out _))
            {
                var exchange = GetExchangeName(TransactionsParsing, _rowId);

                var date = _excelDocument.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, TransactionsParsing, new("Date not found"));

                var currencyCode = _excelDocument.GetCellValue(_rowId, 10);
                if (currencyCode is null)
                    throw new PortfolioCoreException(_initiator, TransactionsParsing, new("Currency not found"));
                var currencyName = currencyCode switch
                {
                    "USD" => Currencies.Usd.ToString(),
                    "Рубль" => Currencies.Rub.ToString(),
                    _ => throw new PortfolioCoreException(_initiator, TransactionsParsing, new("Cirrency not recognized"))
                };

                var sum = _excelDocument.GetCellValue(_rowId, 8);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, TransactionsParsing, new("Sum not found"));

                _transactions.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = expenseValue,
                    Sum = sum,
                    Currency = currencyName,
                    EventType = nameof(EventTypes.Decrease),
                    Info = isin
                });
            }
        }
    }
    private void ParseStockShare(string value, Currencies? currency = null)
    {
        var date = _excelDocument.GetCellValue(_rowId, 4);

        if (date is null)
            throw new PortfolioCoreException(_initiator, StockActionsParsing, new("Date not found"));

        var shareValue = _excelDocument.GetCellValue(_rowId, 7);

        if (shareValue is null)
            throw new PortfolioCoreException(_initiator, StockActionsParsing, new("Value not found"));

        var info = _excelDocument.GetCellValue(_rowId, 12);

        if (info is null)
            throw new PortfolioCoreException(_initiator, StockActionsParsing, new("Information not found"));

        _stockActions.Add(new()
        {
            Ticker = value.Trim(),
            Date = date,
            Value = shareValue,
            Exchange = GetExchangeName(StockActionsParsing, _rowId),
            Info = info,
            EventType = nameof(EventTypes.Increase)
        });
    }
    private void ParseStockSplit(string value, Currencies? currency)
    {
        var date = _excelDocument.GetCellValue(_rowId, 4);

        if (date is null)
            throw new PortfolioCoreException(_initiator, StockActionsParsing, new("Date not found"));

        var valueBefore = _excelDocument.GetCellValue(_rowId, 6);
        if (valueBefore is null || !int.TryParse(valueBefore, out var _valueBefore))
            throw new PortfolioCoreException(_initiator, StockActionsParsing, new("Value before was not recognized"));
        
        var valueAfter = _excelDocument.GetCellValue(_rowId, 7);
        if (valueAfter is null || !int.TryParse(valueAfter, out var _valueAfter))
            throw new PortfolioCoreException(_initiator, StockActionsParsing, new("Value after was not recognized"));

        var info = _excelDocument.GetCellValue(_rowId, 12);

        if (info is null)
            throw new PortfolioCoreException(_initiator, StockActionsParsing, new("Information not found"));

        var splitValue = _valueAfter - _valueBefore;

        _stockActions.Add(new()
        {
            Ticker = value.Trim(),
            Date = date,
            Value = splitValue.ToString(),
            Exchange = GetExchangeName(StockActionsParsing, _rowId),
            Info = info,
            EventType = nameof(EventTypes.Split)
        });
    }

    private ExcelDocument GetExcelDocument(byte[] payload)
    {
        try
        {
            var table = ExcelLoader.LoadTable(payload);
            return new ExcelDocument(table);
        }
        catch (Exception exception)
        {
            throw new PortfolioCoreException(_initiator, nameof(GetExcelDocument), new(exception));
        }
    }
    private Dictionary<string, int> GetFileStructure(int rowId)
    {
        var structure = new Dictionary<string, int>(BcsReportFileStructure.Points.Length);

        while (!_excelDocument.TryGetCellValue(++rowId, 1, "Дата составления отчета:", out var cellValue))
            if (cellValue is not null && BcsReportFileStructure.Points.Any(x => cellValue.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                structure.Add(cellValue, rowId);

        return !structure.Any()
            ? throw new PortfolioCoreException(_initiator, nameof(GetFileStructure), new("The structure of the file was not recognized"))
            : structure;
    }
    private (string DateStart, string DateEnd) GetReportPeriod(int rowId)
    {
        string? period = null;

        while (!_excelDocument.TryGetCellValue(rowId++, 1, "Период:", out _))
            period = _excelDocument.GetCellValue(rowId, 5);

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
    private string GetReportAgreement(int rowId)
    {
        string? agreement = null;

        while (!_excelDocument.TryGetCellValue(rowId++, 1, "Генеральное соглашение:", out _))
            agreement = _excelDocument.GetCellValue(rowId, 5);

        return string.IsNullOrWhiteSpace(agreement)
            ? throw new PortfolioCoreException(_initiator, nameof(GetReportAgreement), new("The agreement was not found"))
            : agreement;
    }
    private string GetExchangeName(string actionName, int rowId)
    {
        for (var columnNo = 10; columnNo < 20; columnNo++)
        {
            var exchange = _excelDocument.GetCellValue(rowId, columnNo);

            if (!string.IsNullOrEmpty(exchange)
                && exchange != "0"
                && !int.TryParse(exchange[0].ToString(), out _)
                && BcsReportFileStructure.ExchangeTypes.ContainsKey(exchange))
                return BcsReportFileStructure.ExchangeTypes[exchange].ToString().ToUpper();
        }

        throw new PortfolioCoreException(_initiator, actionName, new($"The exchange name was not recognized in the row number: {rowId+1}"));
    }
}