using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AM.Services.Portfolio.Domain.DataAccess;
using AM.Services.Portfolio.Domain.DataAccess.Comparators;
using AM.Services.Portfolio.Domain.Entities;
using AM.Services.Portfolio.Domain.Entities.Catalogs;
using AM.Services.Portfolio.Models.Api.Mq;
using ExcelDataReader;
using Microsoft.Extensions.Logging;
using static AM.Services.Common.Contracts.Enums;
using static AM.Services.Common.Contracts.Helpers.LogHelper;
using static AM.Services.Portfolio.Enums;

// ReSharper disable RedundantJumpStatement
// ReSharper disable RedundantAssignment

namespace AM.Services.Portfolio.Services.Data.Reports.Implementations;

public sealed class BcsReportGrabber : IDataGrabber
{
    private const int providerId = (int)Providers.BCS;

    private readonly Repository<Account> accountRepo;
    private readonly Repository<Derivative> derivativeRepo;
    private readonly Repository<Deal> dealRepo;
    private readonly Repository<Event> eventRepo;
    private readonly Repository<Report> reportRepo;
    private readonly ILogger logger;

    public BcsReportGrabber(
        Repository<Account> accountRepo,
        Repository<Derivative> derivativeRepo,
        Repository<Deal> dealRepo,
        Repository<Event> eventRepo,
        Repository<Report> reportRepo,
        ILogger logger)
    {
        this.reportRepo = reportRepo;
        this.dealRepo = dealRepo;
        this.accountRepo = accountRepo;
        this.derivativeRepo = derivativeRepo;
        this.logger = logger;
        this.eventRepo = eventRepo;
    }

    public async Task ProcessAsync(ProviderReportDto report)
    {
        const string methodName = "BCS reports parsing";

        var dbAccounts = await accountRepo.GetSampleAsync(x => x.ProviderId == providerId && x.UserId == report.UserId, x => ValueTuple.Create(x.Name, x.Id));
        var dbDerivatives = await derivativeRepo.GetSampleAsync(x => ValueTuple.Create(x.Id, x.Code));

        var accounts = dbAccounts
            .ToDictionary(x => x.Item1, x => x.Item2);
        var derivatives = dbDerivatives
            .GroupBy(x => x.Item1)
            .ToDictionary(x => x.Key, x => x.Select(y => y.Item2).ToArray());

        try
        {
            var table = GetExcelTable(report);
            var parser = new BcsReportParser(logger, report.UserId, table, accounts, derivatives);

            var account = await accountRepo.FindAsync(x => x.Name == parser.AccountName && x.UserId == report.UserId && x.ProviderId == providerId);

            if (account is null)
                throw new ApplicationException($"Account '{parser.AccountName}' not recognized");

            var reports = await reportRepo.GetSampleAsync(x => x.ProviderId == providerId && x.AccountId == account.Id);

            var alreadyReportDates = reports.SelectMany(x =>
            {
                var date = x.DateStart;
                var daysCount = x.DateEnd.DayNumber - x.DateStart.DayNumber;
                var dates = new List<DateOnly>(daysCount) { date };
                while (daysCount > 0)
                {
                    date = date.AddDays(1);
                    dates.Add(date);
                    daysCount--;
                }
                return dates;
            }).Distinct();

            var deals = parser.Deals.Where(x => !alreadyReportDates.Contains(x.Date)).ToArray();
            var events = parser.Events.Where(x => !alreadyReportDates.Contains(x.Date)).ToArray();

            if (deals.Any())
                _ = await dealRepo.CreateRangeAsync(deals, new DealComparer(), parser.AccountName);
            if (events.Any())
                _ = await eventRepo.CreateRangeAsync(events, new EventComparer(), parser.AccountName);

            if (!deals.Any() && !events.Any())
            {
                logger.LogInfo<BcsReportGrabber>(methodName, report.Name, "no data");
                return;
            }

            var _report = new Report
            {
                Id = report.Name,
                ContentType = report.ContentType,
                Payload = report.Payload,
                ProviderId = providerId,

                AccountId = account.Id,
                DateStart = parser.DateStart,
                DateEnd = parser.DateEnd,
            };
            await reportRepo.CreateAsync(_report, _report.Id);
        }
        catch (Exception exception)
        {
            logger.LogError<BcsReportGrabber>(methodName, exception);
            // automatically creating account
            if (exception.Message.IndexOf("Agreement '", StringComparison.Ordinal) > -1)
            {
                var values = exception.Message.Split('\'');
                var agreement = values[1];
                await accountRepo.CreateAsync(new Account
                {
                    Name = agreement,
                    ProviderId = providerId,
                    UserId = report.UserId
                }, $"{agreement} created automatically");
            }
        }
    }
    private static DataTable GetExcelTable(ProviderReportDto report)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using var stream = new MemoryStream(report.Payload);
        using var reader = ExcelReaderFactory.CreateBinaryReader(stream);
        var dataSet = reader.AsDataSet();
        var table = dataSet.Tables[0];
        stream.Close();
        return table;
    }
}

internal sealed class BcsReportParser
{
    internal string AccountName { get; }
    internal DateOnly DateStart { get; }
    internal DateOnly DateEnd { get; }
    internal List<Deal> Deals { get; }
    internal List<Event> Events { get; }

    private readonly int accountId;
    private readonly ILogger logger;
    private readonly string userId;
    private const int providerId = (int)Providers.BCS;

    private int rowId;
    private readonly DataTable table;

    private readonly IFormatProvider culture;
    private readonly Dictionary<string, int> reportPoints;

    private readonly Dictionary<string, string[]> derivatives;

    private readonly Dictionary<string, (Action<string, Currencies?> Action, EventTypes EventType)> reportActionPatterns;

    internal BcsReportParser(ILogger logger, string userId, DataTable table, Dictionary<string, int> accounts, Dictionary<string, string[]> derivatives)
    {
        this.logger = logger;
        this.userId = userId;
        this.derivatives = derivatives;
        this.table = table;
        culture = new CultureInfo("ru-RU");
        Deals = new List<Deal>(table.Rows.Count);
        Events = new List<Event>(table.Rows.Count);

        reportPoints = new Dictionary<string, int>(BcsReportStructure.Points.Length);

        while (!TryGetCellValue(1, "Дата составления отчета:", 1, out var pointValue))
            if (pointValue is not null && BcsReportStructure.Points.Select(x => pointValue.IndexOf(x, StringComparison.OrdinalIgnoreCase)).Any(x => x > -1))
                reportPoints.Add(pointValue, rowId);

        if (!reportPoints.Any())
            throw new ApplicationException("Report structure not recognized");

        rowId = 0;
        string? _period;
        while (!TryGetCellValue(1, "Период:", 5, out _period))
            continue;

        if (_period is null)
            throw new ApplicationException($"Agreement period '{_period}' not recognized");

        var dates = _period.Split(' ');
        DateStart = DateOnly.Parse(dates[1], culture);
        DateEnd = DateOnly.Parse(dates[3], culture);

        string? _agreement;
        while (!TryGetCellValue(1, "Генеральное соглашение:", 5, out _agreement))
            continue;

        if (_agreement is null || !accounts.ContainsKey(_agreement))
            throw new ApplicationException($"Agreement '{_agreement}' not recognized");

        accountId = accounts[_agreement];
        AccountName = _agreement;

        reportActionPatterns = new(StringComparer.OrdinalIgnoreCase)
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

        Run();
    }

    private void Run()
    {
        string? cellValue;

        var firstBlock = reportPoints.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[0], StringComparison.OrdinalIgnoreCase) > -1);
        if (firstBlock is not null)
        {
            rowId = reportPoints[firstBlock];

            var border = reportPoints.Skip(1).First().Key;

            var rowNo = rowId;
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
                        if (reportActionPatterns.ContainsKey(cellValue))
                            reportActionPatterns[cellValue].Action(cellValue, currency);
                        else if (!BcsReportStructure.SkippedActions.Contains(cellValue, StringComparer.OrdinalIgnoreCase))
                            logger.LogWarning($"parse {firstBlock}", cellValue, "not recognized");
            }
        }

        var secondBlock = reportPoints.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[2], StringComparison.OrdinalIgnoreCase) > -1);
        if (secondBlock is not null)
        {
            rowId = reportPoints[secondBlock] + 3;

            while (!TryGetCellValue(1, "Итого по валюте Рубль:", 1, out cellValue))
                if (!string.IsNullOrWhiteSpace(cellValue))
                    CheckComission(cellValue);
        }

        var thirdBlock = reportPoints.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[3], StringComparison.OrdinalIgnoreCase) > -1);
        if (thirdBlock is not null)
        {
            rowId = reportPoints[thirdBlock];
            var borders = reportPoints.Keys
                .Where(x => BcsReportStructure.Points[4].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1 || BcsReportStructure.Points[5].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1)
                .ToArray();

            while (!TryGetCellValue(1, borders, 6, out cellValue))
                if (!string.IsNullOrWhiteSpace(cellValue) && reportActionPatterns.ContainsKey(cellValue))
                    reportActionPatterns[cellValue].Action(cellValue, null);
        }

        while (!TryGetCellValue(1, "Дата составления отчета:", 12, out cellValue))
            if (!string.IsNullOrWhiteSpace(cellValue) && reportActionPatterns.ContainsKey(cellValue))
                reportActionPatterns[cellValue].Action(GetCellValue(1)!, null);
    }

    private void ParseDividend(string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new ArgumentNullException(nameof(currency));

        var info = GetCellValue(14);

        if (info is null)
            throw new Exception(nameof(ParseDividend) + ". Info not found");

        var dateTime = DateOnly.Parse(GetCellValue(1)!, culture);
        var exchangeId = GetExchangeId();

        // дивиденд

        var derivativeId = currency.Value.ToString().ToUpperInvariant();
        var derivativeCode = derivatives[derivativeId][0];

        Events.Add(new Event
        {
            DerivativeId = derivativeId,
            DerivativeCode = derivativeCode,

            TypeId = (byte)EventTypes.Dividend,
            Value = decimal.Parse(GetCellValue(6)!),

            Date = dateTime,
            Info = info,

            ExchangeId = exchangeId,
            AccountId = accountId,
            UserId = userId,
            ProviderId = providerId
        });

        // комиссия по дивиденду
        decimal tax = 0;
        var taxPosition = info.IndexOf("налог", StringComparison.OrdinalIgnoreCase);
        if (taxPosition > -1)
        {
            var taxRow = info[taxPosition..].Split(' ')[1];
            taxRow = taxRow.IndexOf('$') > -1 ? taxRow[1..] : taxRow;
            tax = decimal.Parse(taxRow, NumberStyles.Number, culture);
        }

        Events.Add(new Event
        {
            DerivativeId = derivativeId,
            DerivativeCode = derivativeCode,

            TypeId = (byte)EventTypes.TaxIncome,
            Value = tax,

            Date = dateTime,
            Info = info,

            ExchangeId = exchangeId,
            AccountId = accountId,
            UserId = userId,
            ProviderId = providerId
        });
    }
    private void ParseComission(string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new ArgumentNullException(nameof(currency));

        var derivativeId = currency.Value.ToString().ToUpperInvariant();
        var derivativeCode = derivatives[derivativeId][0];

        Events.Add(new Event
        {
            DerivativeId = derivativeId,
            DerivativeCode = derivativeCode,

            TypeId = (byte)reportActionPatterns[value].EventType,
            Value = decimal.Parse(GetCellValue(7)!),

            Date = DateOnly.Parse(GetCellValue(1)!, culture),
            Info = value,

            AccountId = accountId,
            UserId = userId,
            ProviderId = providerId,
            ExchangeId = GetExchangeId()
        });
    }
    private void CheckComission(string value)
    {
        if (!reportActionPatterns.ContainsKey(value))
            throw new ApplicationException(nameof(CheckComission) + $".{nameof(EventType)} '{value}' not found");
    }
    private void ParseBalance(string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new ArgumentNullException(nameof(currency));

        var eventType = reportActionPatterns[value].EventType;

        var rowIndex = eventType switch
        {
            EventTypes.Increase => 6,
            EventTypes.Decrease => 7,
            EventTypes.InterestIncome => 6,
            EventTypes.Dividend => 6,
            _ => throw new ApplicationException(nameof(ParseBalance) + $".{nameof(EventType)} '{value}' not recognized")
        };

        var derivativeId = currency.Value.ToString().ToUpperInvariant();
        var derivativeCode = derivatives[derivativeId][0];

        Events.Add(new Event
        {
            DerivativeId = derivativeId,
            DerivativeCode = derivativeCode,

            TypeId = (byte)eventType,
            Value = decimal.Parse(GetCellValue(rowIndex)!),

            Date = DateOnly.Parse(GetCellValue(1)!, culture),
            Info = value,

            AccountId = accountId,
            UserId = userId,
            ProviderId = providerId,
            ExchangeId = GetExchangeId()
        });
    }
    private void ParseExchangeRate(string value, Currencies? currency = null)
    {
        var code = GetCellValue(1);

        if (code is null)
            throw new ApplicationException(nameof(ParseExchangeRate) + ".Code not found");

        if (!BcsReportStructure.ExchangeCurrencies.ContainsKey(code))
            throw new ApplicationException(nameof(ParseExchangeRate) + $".Derivative '{code}' not found");

        var incomeDerivativeId = BcsReportStructure.ExchangeCurrencies[code].Income;
        var incomeDerivativeCode = derivatives[incomeDerivativeId][0];
        var expenseDerivativeId = BcsReportStructure.ExchangeCurrencies[code].Expense;
        var expenseDerivativeCode = derivatives[expenseDerivativeId][0];

        while (!TryGetCellValue(1, $"Итого по {code}:", 5, out var cellBuyValue))
        {
            var date = DateOnly.Parse(GetCellValue(1)!, culture);
            var exchange = GetCellValue(14);
            var exchangeId = exchange is null
                ? throw new ApplicationException(nameof(ParseExchangeRate) + ".Exchange not found")
                : (byte)BcsReportStructure.ExchangeTypes[exchange];

            var dealId = Guid.NewGuid().ToString();
            var _value = 0m;
            var _cost = 0m;

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

                UserId = userId,
                AccountId = accountId,
                ProviderId = providerId,
                ExchangeId = exchangeId
            };

            if (!string.IsNullOrWhiteSpace(cellBuyValue))
            {
                _cost = decimal.Parse(GetCellValue(4)!);
                _value = decimal.Parse(cellBuyValue);

                deal.Cost = _cost;

                deal.Income.Value = _value;
                deal.Income.DerivativeId = incomeDerivativeId;
                deal.Income.DerivativeCode = incomeDerivativeCode;

                deal.Expense.Value = _value * _cost;
                deal.Expense.DerivativeId = expenseDerivativeId;
                deal.Expense.DerivativeCode = expenseDerivativeCode;
            }
            else
            {
                _cost = decimal.Parse(GetCellValue(7)!);
                _value = decimal.Parse(GetCellValue(8)!);

                deal.Cost = _cost;

                deal.Income.Value = _value * _cost;
                deal.Income.DerivativeId = expenseDerivativeId;
                deal.Income.DerivativeCode = expenseDerivativeCode;

                deal.Expense.Value = _value;
                deal.Expense.DerivativeId = incomeDerivativeId;
                deal.Expense.DerivativeCode = incomeDerivativeCode;
            }

            Deals.Add(deal);
        }
    }
    private void ParseStockTransactions(string value, Currencies? currency = null)
    {
        var isin = GetCellValue(7);

        if (isin is null)
            throw new ApplicationException(nameof(ParseStockTransactions) + ".Isin not found");

        var infoArray = isin.Split(',').Select(x => x.Trim());

        var derivativeId = derivatives.Keys.Intersect(infoArray).FirstOrDefault();

        if (derivativeId is null)
            throw new ApplicationException(nameof(ParseStockTransactions) + $".Derivative '{isin}' not found");

        var name = GetCellValue(1);
        while (!TryGetCellValue(1, $"Итого по {name}:", 4, out var cellBuyValue))
        {
            var date = DateOnly.Parse(GetCellValue(1)!, culture);
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

            var derivativeCode = derivatives[derivativeId][0];

            var dealId = Guid.NewGuid().ToString();
            var _value = 0m;
            var _cost = 0m;

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

                AccountId = accountId,
                UserId = userId,
                ProviderId = providerId,
                ExchangeId = exchangeId,
            };

            if (!string.IsNullOrWhiteSpace(cellBuyValue))
            {
                var expenseDerivativeId = currency.Value.ToString().ToUpperInvariant();
                var expenseDerivativeCode = derivatives[expenseDerivativeId][0];

                _cost = decimal.Parse(GetCellValue(5)!);
                _value = decimal.Parse(cellBuyValue);

                deal.Cost = _cost;

                deal.Income.Value = _value;
                deal.Income.DerivativeId = derivativeId;
                deal.Income.DerivativeCode = derivativeCode;

                deal.Expense.Value = _cost * _value;
                deal.Expense.DerivativeId = expenseDerivativeId;
                deal.Expense.DerivativeCode = expenseDerivativeCode;
            }
            else
            {
                var incomeDerivativeId = currency.Value.ToString().ToUpperInvariant();
                var incomeDerivativeCode = derivatives[incomeDerivativeId][0];
                
                _value = decimal.Parse(GetCellValue(7)!);
                _cost = decimal.Parse(GetCellValue(8)!);

                deal.Cost = _cost;

                deal.Income.Value = _cost * _value;
                deal.Income.DerivativeId = incomeDerivativeId;
                deal.Income.DerivativeCode = incomeDerivativeCode;

                deal.Expense.Value = _value;
                deal.Expense.DerivativeId = derivativeId;
                deal.Expense.DerivativeCode = derivativeCode;
            }

            Deals.Add(deal);
        }
    }
    private void ParseAdditionalStockRelease(string value, Currencies? currency = null)
    {
        var ticker = value.Trim();

        var (derivativeId, derivativeCodes) = derivatives.FirstOrDefault(x => x.Value.Contains(ticker, StringComparer.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(derivativeId))
            throw new ApplicationException(nameof(ParseAdditionalStockRelease) + $".Ticker '{ticker}' not found");

        var derivativeCode = derivativeCodes.First(x => x.Equals(ticker, StringComparison.OrdinalIgnoreCase));

        Events.Add(new Event
        {
            DerivativeId = derivativeId,
            DerivativeCode = derivativeCode,

            Value = decimal.Parse(GetCellValue(7)!),

            TypeId = (byte)EventTypes.Increase,

            Date = DateOnly.Parse(GetCellValue(4)!, culture),
            Info = GetCellValue(12) ?? "Не удалось записать дополнительную информацию",

            UserId = userId,
            AccountId = accountId,
            ProviderId = providerId,
            ExchangeId = (byte)Exchanges.SPBEX
        });
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
    private string? GetCellValue(int columnNo)
    {
        var value = table.Rows[rowId].ItemArray[columnNo]?.ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private bool TryGetCellValue(int rowNo, int stopColumnNo, string stopPattern, int targetColumnNo, out string? currentValue)
    {
        var foundingCell = table.Rows[rowNo].ItemArray[stopColumnNo]?.ToString();
        currentValue = table.Rows[rowNo].ItemArray[targetColumnNo]?.ToString();

        return foundingCell is not null && foundingCell.IndexOf(stopPattern, StringComparison.OrdinalIgnoreCase) > -1;
    }
    private bool TryGetCellValue(int stopColumnNo, string stopPattern, int targetColumnNo, out string? currentValue)
    {
        rowId++;

        var foundingCell = table.Rows[rowId].ItemArray[stopColumnNo]?.ToString();
        currentValue = table.Rows[rowId].ItemArray[targetColumnNo]?.ToString();

        return foundingCell is not null && foundingCell.IndexOf(stopPattern, StringComparison.OrdinalIgnoreCase) > -1;
    }
    private bool TryGetCellValue(int stopColumnNo, IEnumerable<string> stopPatterns, int targetColumnNo, out string? currentValue)
    {
        rowId++;

        var foundingCell = table.Rows[rowId].ItemArray[stopColumnNo]?.ToString();
        currentValue = table.Rows[rowId].ItemArray[targetColumnNo]?.ToString();

        return foundingCell is not null
               && stopPatterns
                   .Select(x => foundingCell.IndexOf(x, StringComparison.OrdinalIgnoreCase))
                   .Any(x => x > -1);
    }
}
internal static class BcsReportStructure
{
    internal static readonly string[] Points = {
        "1.1.1. Движение денежных средств по совершенным сделкам (иным операциям) с ценными бумагами",
        "1.2. Займы:",
        "сборы/штрафы (итоговые суммы):",
        "2.1. Сделки:",
        "2.3. Незавершенные сделки",
        "3. Активы:"
    };
    internal static readonly string[] SkippedActions = {
        "Операция",
        "Займы \"овернайт\"",
        "Итого:",
        "Переводы между площадками",
        "Покупка/Продажа",
        "Покупка/Продажа (репо)",
        "Покупка/Продажа (своп)"
    };
    internal static readonly Dictionary<string, Exchanges> ExchangeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "МосБирж(Валютный рынок)", Exchanges.MOEX },
        { "ММВБ", Exchanges.MOEX },
        { "СПБ", Exchanges.SPBEX },
        { "МосБирж(FORTS)", Exchanges.MOEX },
        { "Внебирж.", Exchanges.MOEX }
    };
    internal static readonly Dictionary<string, (string Income, string Expense)> ExchangeCurrencies = new(StringComparer.OrdinalIgnoreCase)
    {
        {  "USDRUB_TOD", ("USD", "RUB") },
        {  "USDRUB_TOM", ("USD","RUB") },
        {  "EURRUB_TOM", ("EUR", "RUB") },
        {  "EURRUB_TOD", ("EUR", "RUB") },
        {  "EURUSD_TOD", ("EUR", "USD")},
        {  "EURUSD_TOM", ("EUR", "USD") }
    };
}