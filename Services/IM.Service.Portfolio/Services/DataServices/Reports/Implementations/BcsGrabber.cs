using ExcelDataReader;

using IM.Service.Common.Net;
using IM.Service.Portfolio.DataAccess.Comparators;
using IM.Service.Portfolio.DataAccess.Entities;
using IM.Service.Portfolio.DataAccess.Repositories;
using IM.Service.Portfolio.Models.Dto.Mq;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IM.Service.Portfolio.DataAccess.Entities.Catalogs;
using static IM.Service.Common.Net.Enums;
using static IM.Service.Portfolio.Enums;
using static IM.Service.Portfolio.Services.DataServices.Reports.Implementations.ReportConfiguration;

// ReSharper disable RedundantJumpStatement
// ReSharper disable RedundantAssignment

namespace IM.Service.Portfolio.Services.DataServices.Reports.Implementations;

public class BcsGrabber : IDataGrabber
{
    private readonly Repository<Account> accountRepo;
    private readonly Repository<Derivative> derivativeRepo;
    private readonly Repository<Deal> dealRepo;
    private readonly Repository<Event> eventRepo;
    private readonly Repository<Report> reportRepo;
    private readonly ILogger<ReportGrabber> logger;

    public BcsGrabber(
        Repository<Account> accountRepo,
        Repository<Derivative> derivativeRepo,
        Repository<Deal> dealRepo,
        Repository<Event> eventRepo,
        Repository<Report> reportRepo,
        ILogger<ReportGrabber> logger)
    {
        this.reportRepo = reportRepo;
        this.dealRepo = dealRepo;
        this.accountRepo = accountRepo;
        this.derivativeRepo = derivativeRepo;
        this.logger = logger;
        this.eventRepo = eventRepo;
    }

    public async Task GrabDataAsync(ReportFileDto file)
    {
        var accounts = await accountRepo.GetSampleAsync(x => x.BrokerId == (byte)Brokers.Bcs && x.UserId == file.UserId, x => x.Name);
        var derevatives = await derivativeRepo.GetSampleAsync(x => ValueTuple.Create(x.Id, x.Code));

        try
        {
            var table = GetExcelTable(file);

            var parser = new ReportParser(file.UserId, table, accounts, derevatives);

            var reports = await reportRepo.GetSampleAsync(x => x.AccountName == parser.AccountName && x.DateStart <= parser.DateEnd);

            var alreadyDates = reports.SelectMany(x =>
            {
                var _date = x.DateStart;
                var _days = x.DateEnd.DayNumber - x.DateStart.DayNumber;
                var _dates = new List<DateOnly>(_days) { _date };
                while (_days > 0)
                {
                    _date = _date.AddDays(1);
                    _dates.Add(_date);
                    _days--;
                }
                return _dates;
            }).Distinct();

            var deals = parser.Deals
                .Where(x => !alreadyDates.Contains(DateOnly.FromDateTime(x.DateTime)))
                .ToList();
            var events = parser.Events
                .Where(x => !alreadyDates.Contains(DateOnly.FromDateTime(x.DateTime)))
                .ToList();

            if (deals.Any())
            {
                foreach (var deal in deals)
                    deal.DateTime = deal.DateTime.ToUniversalTime();

                var (dealError, _) = await dealRepo.CreateAsync(deals, new DealComparer(), nameof(BcsGrabber));

                if (dealError is not null)
                    return;
            }
            else if (events.Any())
            {
                foreach (var _event in events)
                    _event.DateTime = _event.DateTime.ToUniversalTime();

                var (eventError, _) = await eventRepo.CreateAsync(events, new EventComparer(), nameof(BcsGrabber));

                if (eventError is not null)
                    return;
            }
            else
            {
                logger.LogInformation(LogEvents.Processing, "Place: {place}. New transactions was not found.", nameof(BcsGrabber) + '.' + file.Name);
                return;
            }

            await reportRepo.CreateUpdateAsync(new object[] { file.UserId, (byte)Brokers.Bcs, parser.AccountName }, new Report
            {
                AccountName = parser.AccountName,
                DateStart = parser.DateStart,
                DateEnd = parser.DateEnd,
                Name = file.Name,
                ContentType = file.ContentType,
                Payload = file.Payload
            }, file.Name);
        }
        catch (Exception exception)
        {
            logger.LogError(LogEvents.Processing, "Place: {place}. Error: {exception}", nameof(BcsGrabber) + '.' + file.Name, exception.Message);

            // automatically creating account
            if (exception.Message.IndexOf("Agreement '", StringComparison.Ordinal) > -1)
            {
                var values = exception.Message.Split('\'');
                var agreement = values[1];
                await accountRepo.CreateAsync(new Account
                {
                    Name = agreement,
                    BrokerId = (byte)Brokers.Bcs,
                    UserId = file.UserId
                }, agreement);
            }
        }
    }

    private static DataTable GetExcelTable(ReportFileDto file)
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

internal sealed class ReportParser
{
    internal string AccountName { get; }

    internal DateOnly DateStart { get; }
    internal DateOnly DateEnd { get; }

    internal List<Deal> Deals { get; }
    internal List<Event> Events { get; }


    private const byte brokerId = (byte)Brokers.Bcs;

    private readonly IFormatProvider culture;

    private readonly Dictionary<string, int> reportPointPatterns;
    private readonly Dictionary<string, Action<Currencies, string>> reportActionPatterns;

    private readonly Dictionary<string, EventTypes> eventTypes;
    private readonly Dictionary<string, Exchanges> exchangeTypes;
    private readonly Dictionary<string, (Currencies buy, Currencies sell)> exchangeCurrencyTypes;

    private readonly string[] derivativeIds;
    private readonly Dictionary<string, string> derivativeCodes;

    private readonly string userId;
    private readonly DataTable table;
    private int rowId;

    internal ReportParser(string userId, DataTable table, IEnumerable<string> accounts, IEnumerable<(string id, string code)> derivatives)
    {
        this.userId = userId;
        this.table = table;
        culture = new CultureInfo("ru-RU");
        Deals = new List<Deal>(table.Rows.Count);
        Events = new List<Event>(table.Rows.Count);

        reportPointPatterns = new Dictionary<string, int>(ReportPoints.Length);

        while (!TryCellValue(1, "���� ����������� ������:", 1, out var pointValue))
            if (pointValue is not null && ReportPoints.Select(x => pointValue.IndexOf(x, StringComparison.OrdinalIgnoreCase)).Any(x => x > -1))
                reportPointPatterns.Add(pointValue, rowId);

        if (!reportPointPatterns.Any())
            throw new Exception("Report structure was not recognized");

        rowId = 0;
        string? _period;
        while (!TryCellValue(1, "������:", 5, out _period))
            continue;

        if (_period is null)
            throw new Exception($"Agreement period '{_period}' was not recognized");

        var dates = _period.Split(' ');
        DateStart = DateOnly.Parse(dates[1], culture);
        DateEnd = DateOnly.Parse(dates[3], culture);

        string? _agreement;
        while (!TryCellValue(1, "����������� ����������:", 5, out _agreement))
            continue;

        var account = accounts.FirstOrDefault(x => x.Equals(_agreement, StringComparison.OrdinalIgnoreCase));
        AccountName = _agreement is not null && account is not null
                    ? account
                    : throw new Exception($"Agreement '{_agreement}' was not recognized");

        derivatives = derivatives.ToArray();
        derivativeIds = derivatives.Select(x => x.id).ToArray();
        derivativeCodes = derivatives.ToDictionary(x => x.code, y => y.id);

        reportActionPatterns = new(StringComparer.OrdinalIgnoreCase)
        {
            { ReportActions[0], ParseDividend },
            { ReportActions[1], ParseComission },
            { ReportActions[2], ParseComission },
            { ReportActions[3], ParseComission },
            { ReportActions[4], ParseComission },
            { ReportActions[5], ParseComission },
            { ReportActions[6], CheckComission },
            { ReportActions[7], ParseAccountBalance },
            { ReportActions[8], ParseAccountBalance },
            { ReportActions[9], ParseStockTransactions },
            { ReportActions[10], ParseExchangeRate },
            { ReportActions[11], ParseComission },
            { ReportActions[12], ParseComission },
            { ReportActions[13], ParseComission },
            { ReportActions[14], ParseComission },
            { ReportActions[15], ParseComission },
            { ReportActions[16], ParseAdditionalStockRelease }
        };
        eventTypes = new()
        {
            { ReportActions[1], EventTypes.��������_������� },
            { ReportActions[2], EventTypes.��������_������� },
            { ReportActions[3], EventTypes.��������_����������� },
            { ReportActions[4], EventTypes.��������_����������� },
            { ReportActions[5], EventTypes.���� },
            { ReportActions[7], EventTypes.����������_����� },
            { ReportActions[8], EventTypes.�����_�_����� },
            { ReportActions[11], EventTypes.��������_������� },
            { ReportActions[12], EventTypes.��������_������� },
            { ReportActions[13], EventTypes.��������_������� },
            { ReportActions[14], EventTypes.��������_������� },
            { ReportActions[15], EventTypes.��������_������� }
        };
        exchangeTypes = new()
        {
            { ReportExchanges[0], Exchanges.Moex },
            { ReportExchanges[1], Exchanges.Moex },
            { ReportExchanges[2], Exchanges.Spbex }
        };
        exchangeCurrencyTypes = new()
        {
            { ReportExchangeCurrencies[0], (Currencies.Usd, Currencies.Rub) }
        };

        Parse();
    }

    private void Parse()
    {
        string? cellValue;

        var firstBlock = reportPointPatterns.Keys.FirstOrDefault(x => x.IndexOf(ReportPoints[0], StringComparison.OrdinalIgnoreCase) > -1);
        if (firstBlock is not null)
        {
            rowId = reportPointPatterns[firstBlock];

            var border = reportPointPatterns.Skip(1).First().Key;

            var rowNo = rowId;
            while (!TryCellValue(++rowNo, 1, border, 1, out cellValue))
                if (cellValue is not null)
                    switch (cellValue)
                    {
                        case "USD": GetAction(Currencies.Usd, "USD"); break;
                        case "�����": GetAction(Currencies.Rub, "�����"); break;
                    }

            void GetAction(Currencies currency, string value)
            {
                while (!TryCellValue(1, new[] { $"����� �� ������ {value}:", border }, 2, out cellValue))
                    if (cellValue is not null && reportActionPatterns.ContainsKey(cellValue))
                        reportActionPatterns[cellValue](currency, cellValue);
            }
        }

        var secondBlock = reportPointPatterns.Keys.FirstOrDefault(x => x.IndexOf(ReportPoints[2], StringComparison.OrdinalIgnoreCase) > -1);
        if (secondBlock is not null)
        {
            rowId = reportPointPatterns[secondBlock] + 3;

            while (!TryCellValue(1, "����� �� ������ �����:", 1, out cellValue))
                if (cellValue is not null)
                    reportActionPatterns[ReportPoints[2]](Currencies.Default, cellValue);
        }

        var thirdBlock = reportPointPatterns.Keys.FirstOrDefault(x => x.IndexOf(ReportPoints[3], StringComparison.OrdinalIgnoreCase) > -1);
        if (thirdBlock is not null)
        {
            rowId = reportPointPatterns[thirdBlock];
            var borders = reportPointPatterns.Keys
                .Where(x => ReportPoints[4].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1 || ReportPoints[5].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1)
                .ToArray();

            while (!TryCellValue(1, borders, 6, out cellValue))
                if (cellValue is not null && reportActionPatterns.ContainsKey(cellValue))
                    reportActionPatterns[cellValue](Currencies.Default, cellValue);
        }

        while (!TryCellValue(1, "���� ����������� ������:", 12, out cellValue))
            if (cellValue is not null && reportActionPatterns.ContainsKey(cellValue))
                reportActionPatterns[cellValue](Currencies.Default, GetCellValue(1)!);
    }

    private void ParseDividend(Currencies currency, string value)
    {
        var info = GetCellValue(14);

        if (info is null)
            throw new Exception(nameof(ParseDividend) + ".Info about dividend was not found");

        var infoArray = info.Split(',').Select(x => x.Trim());

        var isin = derivativeIds.Intersect(infoArray).FirstOrDefault();

        if (isin is null || !derivativeIds.Contains(isin))
            throw new Exception(nameof(ParseDividend) + $".Isin from '{info}' was not found");

        decimal tax = 0;
        var taxPosition = info.IndexOf("�����", StringComparison.OrdinalIgnoreCase);
        if (taxPosition > -1)
        {
            var taxRow = info[taxPosition..].Split(' ')[1];
            taxRow = taxRow.IndexOf('$') > -1 ? taxRow[1..] : taxRow;
            tax = decimal.Parse(taxRow, NumberStyles.Number, culture);
        }

        var dateTime = DateTime.Parse(GetCellValue(1)!, culture);
        var exchange = GetCellValue(12);
        byte? exchangeId = exchange is null ? null : (byte)exchangeTypes[exchange];

        Events.Add(new Event
        {
            DateTime = dateTime,
            Cost = decimal.Parse(GetCellValue(6)!),
            Info = info,
            EventTypeId = (byte)EventTypes.��������,
            DerivativeId = isin,
            ExchangeId = exchangeId,
            BrokerId = brokerId,
            UserId = userId,
            AccountName = AccountName,
            CurrencyId = (byte)currency
        });
        Events.Add(new Event
        {
            DateTime = dateTime,
            Cost = tax,
            Info = info,
            EventTypeId = (byte)EventTypes.�����_�_���������,
            DerivativeId = isin,
            ExchangeId = exchangeId,
            BrokerId = brokerId,
            UserId = userId,
            AccountName = AccountName,
            CurrencyId = (byte)currency
        });
    }
    private void ParseComission(Currencies currency, string value)
    {
        var info = GetCellValue(14);
        var exchange = GetCellValue(12);
        byte? exchangeId = exchange is null ? null : (byte)exchangeTypes[exchange];

        Events.Add(new Event
        {
            DateTime = DateTime.Parse(GetCellValue(1)!, culture),
            Cost = decimal.Parse(GetCellValue(7)!),
            Info = info,
            EventTypeId = (byte)eventTypes[value],
            ExchangeId = exchangeId,
            BrokerId = brokerId,
            UserId = userId,
            AccountName = AccountName,
            CurrencyId = (byte)currency
        });
    }
    private void CheckComission(Currencies currency, string value)
    {
        if (!reportActionPatterns.ContainsKey(value))
            throw new Exception(nameof(CheckComission) + $".{nameof(EventType)} '{value}' was not found");
    }
    private void ParseAccountBalance(Currencies currency, string value)
    {
        var costRowIndex = eventTypes[value] switch
        {
            EventTypes.����������_����� => 6,
            EventTypes.�����_�_����� => 7,
            _ => throw new ArgumentOutOfRangeException(nameof(ParseAccountBalance) + $".{nameof(EventType)} was not recognized")
        };

        var exchange = GetCellValue(12);
        byte? exchangeId = exchange is null ? null : (byte)exchangeTypes[exchange];

        Events.Add(new Event
        {
            DateTime = DateTime.Parse(GetCellValue(1)!, culture),
            Cost = decimal.Parse(GetCellValue(costRowIndex)!),
            Info = value,
            EventTypeId = (byte)eventTypes[value],
            ExchangeId = exchangeId,
            BrokerId = brokerId,
            UserId = userId,
            AccountName = AccountName,
            CurrencyId = (byte)currency
        });
    }
    private void ParseExchangeRate(Currencies currency, string value)
    {
        var name = GetCellValue(1);

        if (name is null)
            throw new Exception(nameof(ParseExchangeRate) + ".Exchange rate code was not found");

        if (!derivativeCodes.ContainsKey(name))
            throw new Exception(nameof(ParseExchangeRate) + $".Exchange rate code '{name}' was not found");

        var buyCurrencyId = (byte)exchangeCurrencyTypes[name].buy;
        var sellCurrencyId = (byte)exchangeCurrencyTypes[name].sell;

        while (!TryCellValue(1, $"����� �� {name}:", 5, out var cellBuyValue))
        {
            var date = DateTime.Parse(GetCellValue(1)!, culture);
            var exchange = GetCellValue(14);
            var exchangeId = exchange is null
                ? throw new Exception(nameof(ParseExchangeRate) + ".Exchange was not recognized")
                : (byte)exchangeTypes[exchange];

            if (!string.IsNullOrWhiteSpace(cellBuyValue))
                Deals.Add(new Deal
                {
                    DateTime = date,
                    Cost = decimal.Parse(GetCellValue(4)!),
                    Value = decimal.Parse(cellBuyValue),
                    Info = name,
                    DerivativeId = derivativeCodes[name],
                    ExchangeId = exchangeId,
                    BrokerId = brokerId,
                    UserId = userId,
                    AccountName = AccountName,
                    OperationId = (byte)Operations.������,
                    CurrencyId = buyCurrencyId
                });
            else
                Deals.Add(new Deal
                {
                    DateTime = date,
                    Cost = decimal.Parse(GetCellValue(7)!),
                    Value = decimal.Parse(GetCellValue(8)!),
                    Info = name,
                    DerivativeId = derivativeCodes[name],
                    ExchangeId = exchangeId,
                    BrokerId = brokerId,
                    UserId = userId,
                    AccountName = AccountName,
                    OperationId = (byte)Operations.������,
                    CurrencyId = sellCurrencyId
                });
        }
    }
    private void ParseStockTransactions(Currencies currency, string value)
    {
        var name = GetCellValue(1);
        var isin = GetCellValue(7);

        if (isin is null)
            throw new Exception(nameof(ParseStockTransactions) + ".Isin was not found");

        var infoArray = isin.Split(',').Select(x => x.Trim());

        var _isin = derivativeIds.Intersect(infoArray).FirstOrDefault();

        if (_isin is null || !derivativeIds.Contains(_isin))
            throw new Exception(nameof(ParseStockTransactions) + $".Isin '{isin}' was not found");

        while (!TryCellValue(1, $"����� �� {name}:", 4, out var cellBuyValue))
        {
            var date = DateTime.Parse(GetCellValue(1)!, culture);
            currency = GetCellValue(10) switch
            {
                "USD" => Currencies.Usd,
                "�����" => Currencies.Rub,
                _ => throw new ArgumentOutOfRangeException(nameof(ParseStockTransactions) + '.' + " Currency was not found")
            };

            var exchange = GetCellValue(17);
            var exchangeId = exchange is null
                ? throw new Exception(nameof(ParseStockTransactions) + ".Exchange was not recognized")
                : (byte)exchangeTypes[exchange];

            if (!string.IsNullOrWhiteSpace(cellBuyValue))
                Deals.Add(new Deal
                {
                    DateTime = date,
                    Cost = decimal.Parse(GetCellValue(5)!),
                    Value = decimal.Parse(cellBuyValue),
                    Info = name,
                    DerivativeId = isin,
                    ExchangeId = exchangeId,
                    BrokerId = brokerId,
                    UserId = userId,
                    AccountName = AccountName,
                    OperationId = (byte)Operations.������,
                    CurrencyId = (byte)currency
                });
            else
                Deals.Add(new Deal
                {
                    DateTime = date,
                    Cost = decimal.Parse(GetCellValue(8)!),
                    Value = decimal.Parse(GetCellValue(7)!),
                    Info = name,
                    DerivativeId = isin,
                    ExchangeId = exchangeId,
                    BrokerId = brokerId,
                    UserId = userId,
                    AccountName = AccountName,
                    OperationId = (byte)Operations.������,
                    CurrencyId = (byte)currency
                });
        }
    }
    private void ParseAdditionalStockRelease(Currencies currency, string value)
    {
        var ticker = value.Trim();
        
        if (!derivativeCodes.ContainsKey(ticker))
            throw new Exception(nameof(ParseAdditionalStockRelease) + $".Ticker '{ticker}' was not found");

        Deals.Add(new Deal
        {
            DateTime = DateTime.Parse(GetCellValue(4)!, culture),
            Cost = 0,
            Value = decimal.Parse(GetCellValue(7)!),
            Info = GetCellValue(12),
            DerivativeId = derivativeCodes[ticker],
            ExchangeId = 0,
            BrokerId = brokerId,
            UserId = userId,
            AccountName = AccountName,
            OperationId = (byte)Operations.������,
            CurrencyId = (byte)currency
        });
    }

    private bool TryCellValue(int rowNo, int stopColumnNo, string stopPattern, int targetColumnNo, out string? currentValue)
    {
        var foundingCell = table.Rows[rowNo].ItemArray[stopColumnNo]?.ToString();
        currentValue = table.Rows[rowNo].ItemArray[targetColumnNo]?.ToString();

        return foundingCell is not null && foundingCell.IndexOf(stopPattern, StringComparison.OrdinalIgnoreCase) > -1;
    }
    private bool TryCellValue(int stopColumnNo, string stopPattern, int targetColumnNo, out string? currentValue)
    {
        rowId++;

        var foundingCell = table.Rows[rowId].ItemArray[stopColumnNo]?.ToString();
        currentValue = table.Rows[rowId].ItemArray[targetColumnNo]?.ToString();

        return foundingCell is not null && foundingCell.IndexOf(stopPattern, StringComparison.OrdinalIgnoreCase) > -1;
    }
    private bool TryCellValue(int stopColumnNo, IEnumerable<string> stopPatterns, int targetColumnNo, out string? currentValue)
    {
        rowId++;

        var foundingCell = table.Rows[rowId].ItemArray[stopColumnNo]?.ToString();
        currentValue = table.Rows[rowId].ItemArray[targetColumnNo]?.ToString();

        return foundingCell is not null
               && stopPatterns
                   .Select(x => foundingCell
                       .IndexOf(x, StringComparison.OrdinalIgnoreCase))
                   .Any(x => x > -1);
    }
    private string? GetCellValue(int columnNo) => table.Rows[rowId].ItemArray[columnNo]?.ToString();
}
internal static class ReportConfiguration
{
    internal static readonly string[] ReportPoints = {
        "1.1.1. �������� �������� ������� �� ����������� ������� (���� ���������) � ������� ��������",
        "1.2. �����:",
        "�����/������ (�������� �����):",
        "2.1. ������:",
        "2.3. ������������� ������",
        "3. ������:"
    };
    internal static readonly string[] ReportActions = {
        "���������",
        "�������������� ������",
        "�������������� ��������",
        "�������������� �� ������������ ����� ����",
        "�������� ��",
        "����",
        ReportPoints[2],
        "������ ��",
        "����� ��",
        "ISIN:",
        "������. ������:",
        "�������������� �������� (����)",
        "�������� �� ����� \"�������� ��\"",
        "�������������� �������� (����)",
        "�������� �������� ����",
        "������ �� ����� �������� �������",
        "���. ������ ����� "
    };
    internal static readonly string[] ReportExchanges =
    {
        "�������(�������� �����)",
        "����",
        "���"
    };
    internal static readonly string[] ReportExchangeCurrencies =
    {
        "USDRUB_TOD"
    };
}