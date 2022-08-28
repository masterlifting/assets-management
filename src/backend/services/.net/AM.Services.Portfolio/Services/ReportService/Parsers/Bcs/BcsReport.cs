using AM.Services.Common.Contracts.Helpers;
using AM.Services.Portfolio.Domain.Entities;
using AM.Services.Portfolio.Domain.Entities.Catalogs;

using ExcelDataReader;

using Microsoft.Extensions.Logging;

using Shared.Core.Exceptions;

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using static AM.Services.Common.Contracts.Enums;
using static AM.Services.Portfolio.Enums;

namespace AM.Services.Portfolio.Services.ReportService.Parsers.Bcs;

public sealed class BcsReport
{
    public ReportFile ReportFile { get; }
    public BcsReportHeader? Header { get; set; }
    public BcsReportBody? Body { get; set; }

    private const int ProviderId = (int)Providers.BCS;

    private readonly IFormatProvider _culture;
    private readonly ILogger _logger;

    private int _rowId;
    private readonly DataTable _table;

    private readonly Dictionary<string, string[]> _derivatives;
    private readonly Dictionary<string, int> _reportPoints;
    private readonly Dictionary<string, (Action<BcsReportBody, string, Currencies?> Action, EventTypes EventType)> _reportPatterns;

    public BcsReport(ILogger logger, ReportFile file, Dictionary<string, string[]> derivatives)
    {
        ReportFile = file;

        try
        {
            _table = GetExcelTable(file);
        }
        catch (Exception exception)
        {
            throw new SharedCoreProcessException("разбор отчета БКС", "получение excel таблицы отчета", exception.Message);
        }

        _logger = logger;
        _derivatives = derivatives;
        _culture = new CultureInfo("ru-RU");
        _reportPoints = new Dictionary<string, int>(BcsReportStructure.Points.Length);
        _reportPatterns = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Дивиденды", (ParseDividend, EventTypes.Dividend) },
            { "Урегулирование сделок", (ParseComission, EventTypes.TaxProvider) },
            { "Вознаграждение компании", (ParseComission, EventTypes.TaxProvider) },
            { "Вознаграждение за обслуживание счета депо", (ParseComission, EventTypes.TaxDepositary) },
            { "Хранение ЦБ", (ParseComission, EventTypes.TaxDepositary) },
            { "НДФЛ", (ParseComission, EventTypes.NDFL) },
            { "Приход ДС", (ParseBalance, EventTypes.Increase) },
            { "Вывод ДС", (ParseBalance, EventTypes.Decrease) },
            { "ISIN:", (ParseStockTransactions, EventTypes.Default) },
            { "Сопряж. валюта:", (ParseExchangeRate, EventTypes.Default) },
            { "Вознаграждение компании (СВОП)", (ParseComission, EventTypes.TaxProvider) },
            { "Комиссия за займы \"овернайт ЦБ\"", (ParseComission, EventTypes.TaxProvider) },
            { "Вознаграждение компании (репо)", (ParseComission, EventTypes.TaxProvider) },
            { "Комиссия Биржевой гуру", (ParseComission, EventTypes.TaxProvider) },
            { "Оплата за вывод денежных средств", (ParseComission, EventTypes.TaxProvider) },
            { "Доп. выпуск акций ", (ParseAdditionalStockRelease, EventTypes.Increase) },
            { "Проценты по займам \"овернайт ЦБ\"", (ParseBalance, EventTypes.InterestIncome) },
            { "Проценты по займам \"овернайт\"", (ParseBalance, EventTypes.InterestIncome) },
            { "Распределение (4*)", (ParseComission, EventTypes.TaxDepositary) },
            { "Возмещение дивидендов по сделке", (ParseBalance, EventTypes.Dividend) }
        };
    }

    public BcsReportHeader GetHeader()
    {
        while (!TryGetCellValue(1, "Дата составления отчета:", 1, out var pointValue))
            if (pointValue is not null && BcsReportStructure.Points.Select(x => pointValue.IndexOf(x, StringComparison.OrdinalIgnoreCase)).Any(x => x > -1))
                _reportPoints.Add(pointValue, _rowId);

        if (!_reportPoints.Any())
            throw new ApplicationException("Report structure not recognized");

        string? period;
        while (!TryGetCellValue(1, "Период:", 5, out period))
            // ReSharper disable once RedundantJumpStatement
            continue;

        if (period is null)
            throw new ApplicationException($"Agreement period '{period}' not recognized");
        var dates = period.Split(' ');

        string? agreement;
        while (!TryGetCellValue(1, "Генеральное соглашение:", 5, out agreement))
            // ReSharper disable once RedundantJumpStatement
            continue;

        return new BcsReportHeader
        {
            Agreement = agreement ?? throw new ApplicationException($"Agreement '{agreement}' not found"),
            DateStart = DateOnly.Parse(dates[1], _culture),
            DateEnd = DateOnly.Parse(dates[3], _culture)
        };
    }
    public BcsReportBody GetBody(int accountId)
    {
        var body = new BcsReportBody(accountId, _table.Rows.Count);

        string? cellValue;

        var firstBlock = _reportPoints.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[0], StringComparison.OrdinalIgnoreCase) > -1);
        if (firstBlock is not null)
        {
            _rowId = _reportPoints[firstBlock];

            var border = _reportPoints.Skip(1).First().Key;

            var rowNo = _rowId;
            while (!TryGetCellValue(++rowNo, 1, border, 1, out cellValue))
                if (!string.IsNullOrWhiteSpace(cellValue))
                    switch (cellValue)
                    {
                        case "USD": GetAction("USD", Currencies.USD); break;
                        case "Рубль": GetAction("Рубль", Currencies.RUB); break;
                    }

            void GetAction(string value, Currencies? currency)
            {
                while (!TryGetCellValue(1, new[] { $"Итого по валюте {value}:", border }, 2, out cellValue))
                    if (!string.IsNullOrWhiteSpace(cellValue))
                        if (_reportPatterns.ContainsKey(cellValue))
                            _reportPatterns[cellValue].Action(body, cellValue, currency);
                        else if (!BcsReportStructure.SkippedActions.Contains(cellValue, StringComparer.OrdinalIgnoreCase))
                            _logger.LogWarning($"parse {firstBlock}", cellValue, "not recognized");
            }
        }

        var secondBlock = _reportPoints.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[2], StringComparison.OrdinalIgnoreCase) > -1);
        if (secondBlock is not null)
        {
            _rowId = _reportPoints[secondBlock] + 3;

            while (!TryGetCellValue(1, "Итого по валюте Рубль:", 1, out cellValue))
                if (!string.IsNullOrWhiteSpace(cellValue))
                    CheckComission(cellValue);
        }

        var thirdBlock = _reportPoints.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[3], StringComparison.OrdinalIgnoreCase) > -1);
        if (thirdBlock is not null)
        {
            _rowId = _reportPoints[thirdBlock];
            var borders = _reportPoints.Keys
                .Where(x => BcsReportStructure.Points[4].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1 || BcsReportStructure.Points[5].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1)
                .ToArray();

            while (!TryGetCellValue(1, borders, 6, out cellValue))
                if (!string.IsNullOrWhiteSpace(cellValue) && _reportPatterns.ContainsKey(cellValue))
                    _reportPatterns[cellValue].Action(body, cellValue, null);
        }

        while (!TryGetCellValue(1, "Дата составления отчета:", 12, out cellValue))
            if (!string.IsNullOrWhiteSpace(cellValue) && _reportPatterns.ContainsKey(cellValue))
                _reportPatterns[cellValue].Action(body, GetCellValue(1)!, null);

        return body;
    }

    private void ParseDividend(BcsReportBody body, string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new ArgumentNullException(nameof(currency));

        var info = GetCellValue(14);

        if (info is null)
            throw new Exception(nameof(ParseDividend) + ". Info not found");

        var dateTime = DateOnly.Parse(GetCellValue(1)!, _culture);
        var exchangeId = GetExchangeId();

        // дивиденд

        var derivativeId = currency.Value.ToString().ToUpperInvariant();
        var derivativeCode = _derivatives[derivativeId][0];

        body.AddEvent(new Event
        {
            DerivativeId = derivativeId,
            DerivativeCode = derivativeCode,

            TypeId = (byte)EventTypes.Dividend,
            Value = decimal.Parse(GetCellValue(6)!),

            Date = dateTime,
            Info = info,

            ExchangeId = exchangeId,
            AccountId = body.AccountId,
            UserId = ReportFile.UserId,
            ProviderId = ProviderId
        });

        // комиссия по дивиденду
        decimal tax = 0;
        var taxPosition = info.IndexOf("налог", StringComparison.OrdinalIgnoreCase);
        if (taxPosition > -1)
        {
            var taxRow = info[taxPosition..].Split(' ')[1];
            taxRow = taxRow.IndexOf('$') > -1 ? taxRow[1..] : taxRow;
            tax = decimal.Parse(taxRow, NumberStyles.Number, _culture);
        }

        body.AddEvent(new Event
        {
            DerivativeId = derivativeId,
            DerivativeCode = derivativeCode,

            TypeId = (byte)EventTypes.TaxIncome,
            Value = tax,

            Date = dateTime,
            Info = info,

            ExchangeId = exchangeId,
            AccountId = body.AccountId,
            UserId = ReportFile.UserId,
            ProviderId = ProviderId
        });
    }
    private void ParseComission(BcsReportBody body, string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new ArgumentNullException(nameof(currency));

        var derivativeId = currency.Value.ToString().ToUpperInvariant();
        var derivativeCode = _derivatives[derivativeId][0];

        body.AddEvent(new Event
        {
            DerivativeId = derivativeId,
            DerivativeCode = derivativeCode,

            TypeId = (byte)_reportPatterns[value].EventType,
            Value = decimal.Parse(GetCellValue(7)!),

            Date = DateOnly.Parse(GetCellValue(1)!, _culture),
            Info = value,

            AccountId = body.AccountId,
            UserId = ReportFile.UserId,
            ProviderId = ProviderId,
            ExchangeId = GetExchangeId()
        });
    }
    private void ParseBalance(BcsReportBody body, string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new ArgumentNullException(nameof(currency));

        var eventType = _reportPatterns[value].EventType;

        var rowIndex = eventType switch
        {
            EventTypes.Increase => 6,
            EventTypes.Decrease => 7,
            EventTypes.InterestIncome => 6,
            EventTypes.Dividend => 6,
            _ => throw new ApplicationException(nameof(ParseBalance) + $".{nameof(EventType)} '{value}' not recognized")
        };

        var derivativeId = currency.Value.ToString().ToUpperInvariant();
        var derivativeCode = _derivatives[derivativeId][0];

        body.AddEvent(new Event
        {
            DerivativeId = derivativeId,
            DerivativeCode = derivativeCode,

            TypeId = (byte)eventType,
            Value = decimal.Parse(GetCellValue(rowIndex)!),

            Date = DateOnly.Parse(GetCellValue(1)!, _culture),
            Info = value,

            AccountId = body.AccountId,
            UserId = ReportFile.UserId,
            ProviderId = ProviderId,
            ExchangeId = GetExchangeId()
        });
    }
    private void ParseExchangeRate(BcsReportBody body, string value, Currencies? currency = null)
    {
        var code = GetCellValue(1);

        if (code is null)
            throw new ApplicationException(nameof(ParseExchangeRate) + ".Code not found");

        if (!BcsReportStructure.ExchangeCurrencies.ContainsKey(code))
            throw new ApplicationException(nameof(ParseExchangeRate) + $".Derivative '{code}' not found");

        var incomeDerivativeId = BcsReportStructure.ExchangeCurrencies[code].Income;
        var incomeDerivativeCode = _derivatives[incomeDerivativeId][0];
        var expenseDerivativeId = BcsReportStructure.ExchangeCurrencies[code].Expense;
        var expenseDerivativeCode = _derivatives[expenseDerivativeId][0];

        while (!TryGetCellValue(1, $"Итого по {code}:", 5, out var cellBuyValue))
        {
            var date = DateOnly.Parse(GetCellValue(1)!, _culture);
            var exchange = GetCellValue(14);
            var exchangeId = exchange is null
                ? throw new ApplicationException(nameof(ParseExchangeRate) + ".Exchange not found")
                : (byte)BcsReportStructure.ExchangeTypes[exchange];

            var dealId = Guid.NewGuid().ToString();
            decimal dealValue;
            decimal dealCost;

            var deal = new Deal
            {
                Id = dealId,
                Date = date,
                Info = code,

                Income = new Income
                {
                    DealId = dealId,
                    Date = date,
                },
                Expense = new Expense
                {
                    DealId = dealId,
                    Date = date,
                },

                UserId = ReportFile.UserId,
                AccountId = body.AccountId,
                ProviderId = ProviderId,
                ExchangeId = exchangeId
            };

            if (!string.IsNullOrWhiteSpace(cellBuyValue))
            {
                dealCost = decimal.Parse(GetCellValue(4)!);
                dealValue = decimal.Parse(cellBuyValue);

                deal.Cost = dealCost;

                deal.Income.Value = dealValue;
                deal.Income.DerivativeId = incomeDerivativeId;
                deal.Income.DerivativeCode = incomeDerivativeCode;

                deal.Expense.Value = dealValue * dealCost;
                deal.Expense.DerivativeId = expenseDerivativeId;
                deal.Expense.DerivativeCode = expenseDerivativeCode;
            }
            else
            {
                dealCost = decimal.Parse(GetCellValue(7)!);
                dealValue = decimal.Parse(GetCellValue(8)!);

                deal.Cost = dealCost;

                deal.Income.Value = dealValue * dealCost;
                deal.Income.DerivativeId = expenseDerivativeId;
                deal.Income.DerivativeCode = expenseDerivativeCode;

                deal.Expense.Value = dealValue;
                deal.Expense.DerivativeId = incomeDerivativeId;
                deal.Expense.DerivativeCode = incomeDerivativeCode;
            }

            body.AddDeal(deal);
        }
    }
    private void ParseStockTransactions(BcsReportBody body, string value, Currencies? currency = null)
    {
        var isin = GetCellValue(7);

        if (isin is null)
            throw new ApplicationException(nameof(ParseStockTransactions) + ".Isin not found");

        var infoArray = isin.Split(',').Select(x => x.Trim());

        var derivativeId = _derivatives.Keys.Intersect(infoArray).FirstOrDefault();

        if (derivativeId is null)
            throw new ApplicationException(nameof(ParseStockTransactions) + $".Derivative '{isin}' not found");

        var name = GetCellValue(1);
        while (!TryGetCellValue(1, $"Итого по {name}:", 4, out var cellBuyValue))
        {
            var date = DateOnly.Parse(GetCellValue(1)!, _culture);
            currency = GetCellValue(10) switch
            {
                "USD" => Currencies.USD,
                "Рубль" => Currencies.RUB,
                _ => throw new ArgumentOutOfRangeException(nameof(ParseStockTransactions) + $".Currency {currency} not found")
            };

            var exchange = GetCellValue(17);
            var exchangeId = exchange is null
                ? throw new ApplicationException(nameof(ParseStockTransactions) + ".Exchange not found")
                : (byte)BcsReportStructure.ExchangeTypes[exchange];

            var derivativeCode = _derivatives[derivativeId][0];

            var dealId = Guid.NewGuid().ToString();
            decimal dealValue;
            decimal dealCost;

            var deal = new Deal
            {
                Id = dealId,
                Date = date,
                Info = name ?? "Не удалось записать дополнительную информацию",

                Income = new Income
                {
                    DealId = dealId,
                    Date = date
                },
                Expense = new Expense
                {
                    DealId = dealId,
                    Date = date
                },

                AccountId = body.AccountId,
                UserId = ReportFile.UserId,
                ProviderId = ProviderId,
                ExchangeId = exchangeId,
            };

            if (!string.IsNullOrWhiteSpace(cellBuyValue))
            {
                var expenseDerivativeId = currency.Value.ToString().ToUpperInvariant();
                var expenseDerivativeCode = _derivatives[expenseDerivativeId][0];

                dealCost = decimal.Parse(GetCellValue(5)!);
                dealValue = decimal.Parse(cellBuyValue);

                deal.Cost = dealCost;

                deal.Income.Value = dealValue;
                deal.Income.DerivativeId = derivativeId;
                deal.Income.DerivativeCode = derivativeCode;

                deal.Expense.Value = dealCost * dealValue;
                deal.Expense.DerivativeId = expenseDerivativeId;
                deal.Expense.DerivativeCode = expenseDerivativeCode;
            }
            else
            {
                var incomeDerivativeId = currency.Value.ToString().ToUpperInvariant();
                var incomeDerivativeCode = _derivatives[incomeDerivativeId][0];

                dealValue = decimal.Parse(GetCellValue(7)!);
                dealCost = decimal.Parse(GetCellValue(8)!);

                deal.Cost = dealCost;

                deal.Income.Value = dealCost * dealValue;
                deal.Income.DerivativeId = incomeDerivativeId;
                deal.Income.DerivativeCode = incomeDerivativeCode;

                deal.Expense.Value = dealValue;
                deal.Expense.DerivativeId = derivativeId;
                deal.Expense.DerivativeCode = derivativeCode;
            }

            body.AddDeal(deal);
        }
    }
    private void ParseAdditionalStockRelease(BcsReportBody body, string value, Currencies? currency = null)
    {
        var ticker = value.Trim();

        var (derivativeId, derivativeCodes) = _derivatives.FirstOrDefault(x => x.Value.Contains(ticker, StringComparer.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(derivativeId))
            throw new ApplicationException(nameof(ParseAdditionalStockRelease) + $".Ticker '{ticker}' not found");

        var derivativeCode = derivativeCodes.First(x => x.Equals(ticker, StringComparison.OrdinalIgnoreCase));

        body.AddEvent(new Event
        {
            DerivativeId = derivativeId,
            DerivativeCode = derivativeCode,

            Value = decimal.Parse(GetCellValue(7)!),

            TypeId = (byte)EventTypes.Increase,

            Date = DateOnly.Parse(GetCellValue(4)!, _culture),
            Info = GetCellValue(12) ?? "Не удалось записать дополнительную информацию",

            UserId = ReportFile.UserId,
            AccountId = body.AccountId,
            ProviderId = ProviderId,
            ExchangeId = (byte)Exchanges.SPBEX
        });
    }

    private void CheckComission(string value)
    {
        if (!_reportPatterns.ContainsKey(value))
            throw new ApplicationException(nameof(CheckComission) + $".{nameof(EventType)} '{value}' not found");
    }
    private byte GetExchangeId()
    {
        var exchange = GetCellValue(12);
        if (string.IsNullOrWhiteSpace(exchange))
            exchange = GetCellValue(11);
        if (string.IsNullOrWhiteSpace(exchange))
            exchange = GetCellValue(10);

        return string.IsNullOrWhiteSpace(exchange) ? (byte)Exchanges.Default : (byte)BcsReportStructure.ExchangeTypes[exchange];

    }
    private bool TryGetCellValue(int stopColumnNo, string stopPattern, int targetColumnNo, out string? currentValue)
    {
        _rowId++;

        var foundingCell = _table.Rows[_rowId].ItemArray[stopColumnNo]?.ToString();
        currentValue = _table.Rows[_rowId].ItemArray[targetColumnNo]?.ToString();

        return foundingCell is not null && foundingCell.IndexOf(stopPattern, StringComparison.OrdinalIgnoreCase) > -1;
    }
    private bool TryGetCellValue(int stopColumnNo, IEnumerable<string> stopPatterns, int targetColumnNo, out string? currentValue)
    {
        _rowId++;

        var foundingCell = _table.Rows[_rowId].ItemArray[stopColumnNo]?.ToString();
        currentValue = _table.Rows[_rowId].ItemArray[targetColumnNo]?.ToString();

        return foundingCell is not null
               && stopPatterns
                   .Select(x => foundingCell.IndexOf(x, StringComparison.OrdinalIgnoreCase))
                   .Any(x => x > -1);
    }

    private string? GetCellValue(int columnNo)
    {
        var value = _table.Rows[_rowId].ItemArray[columnNo]?.ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
    private bool TryGetCellValue(int rowNo, int stopColumnNo, string stopPattern, int targetColumnNo, out string? result)
    {
        var foundingCell = _table.Rows[rowNo].ItemArray[stopColumnNo]?.ToString();
        result = _table.Rows[rowNo].ItemArray[targetColumnNo]?.ToString();

        return foundingCell is not null && foundingCell.IndexOf(stopPattern, StringComparison.OrdinalIgnoreCase) > -1;
    }
    private static DataTable GetExcelTable(ReportFile file)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using var stream = new MemoryStream(file.Payload);
        using var reader = ExcelReaderFactory.CreateBinaryReader(stream);
        var dataSet = reader.AsDataSet();
        var table = dataSet.Tables[0];
        stream.Close();
        return table;
    }
}

public class BcsReportHeader
{
    public string Agreement { get; init; } = null!;
    public DateOnly DateStart { get; init; }
    public DateOnly DateEnd { get; init; }
}

public class BcsReportBody
{
    public int AccountId { get; }
    public IEnumerable<Deal> Deals { get; }
    public IEnumerable<Event> Events { get; }

    private readonly List<Deal> _deals;
    private readonly List<Event> _events;
    public BcsReportBody(int accountId, int capacity)
    {
        AccountId = accountId;
        
        _deals = new List<Deal>(capacity);
        _events = new List<Event>(capacity);

        Deals = _deals;
        Events = _events;
    }

    public void AddDeal(Deal bcsDeal) => _deals.Add(bcsDeal);
    public void AddEvent(Event bcsEvent) => _events.Add(bcsEvent);
}