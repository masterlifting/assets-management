using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Models;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Deserialization.Reports.Bcs.Models;

using Microsoft.Extensions.Logging;

using Shared.Data.Excel;

using System.Globalization;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Deserialization.Reports.Bcs;

public sealed class BcsReport
{
    private const string Initiator = "������� ��� ������ (excel)";
    private const string DividendAction = "����� ����������";
    private const string ComissionAction = "����� ��������";
    private const string BalanceAction = "����� �������� �������� ������� �� �������";
    private const string ExchangeRateAction = "����� ����� �����";

    private readonly ExcelDocument _excelDocument;
    private int _rowId;

    private readonly List<BcsReportDividendModel> _dividends;
    private readonly List<BcsReportComissionModel> _comissions;
    private readonly List<BcsReportBalanceModel> _balances;
    private readonly List<BcsReportExchangeRateModel> _exchangeRates;
    private readonly List<BcsReportTransactionModel> _transactions;
    private readonly List<BcsReportStockMoveModel> _stockMoves;

    private readonly Dictionary<string, (Action<string, Currencies?> Action, EventTypes EventType)> _reportPatterns;

    public BcsReport(byte[] payload)
    {
        _excelDocument = GetExcelDocument(payload);

        _reportPatterns = new(StringComparer.OrdinalIgnoreCase)
        {
            { "���������", (ParseDividend, EventTypes.Dividend) },
            { "�������������� ������", (ParseComission, EventTypes.TaxProvider) },
            { "�������������� ��������", (ParseComission, EventTypes.TaxProvider) },
            { "�������������� �� ������������ ����� ����", (ParseComission, EventTypes.TaxDepositary) },
            { "�������� ��", (ParseComission, EventTypes.TaxDepositary) },
            { "����", (ParseComission, EventTypes.Ndfl) },
            { "������ ��", (ParseBalance, EventTypes.Increase) },
            { "����� ��", (ParseBalance, EventTypes.Decrease) },
            { "ISIN:", (ParseStockTransactions, EventTypes.Default) },
            { "������. ������:", (ParseExchangeRate, EventTypes.Default) },
            { "�������������� �������� (����)", (ParseComission, EventTypes.TaxProvider) },
            { "�������� �� ����� \"�������� ��\"", (ParseComission, EventTypes.TaxProvider) },
            { "�������������� �������� (����)", (ParseComission, EventTypes.TaxProvider) },
            { "�������� �������� ����", (ParseComission, EventTypes.TaxProvider) },
            { "������ �� ����� �������� �������", (ParseComission, EventTypes.TaxProvider) },
            { "���. ������ ����� ", (ParseAdditionalStockRelease, EventTypes.Increase) },
            { "�������� �� ������ \"�������� ��\"", (ParseBalance, EventTypes.InterestIncome) },
            { "�������� �� ������ \"��������\"", (ParseBalance, EventTypes.InterestIncome) },
            { "������������� (4*)", (ParseComission, EventTypes.TaxDepositary) },
            { "���������� ���������� �� ������", (ParseBalance, EventTypes.Dividend) }
        };

        _dividends = new List<BcsReportDividendModel>(_excelDocument.RowsCount);
        _comissions = new List<BcsReportComissionModel>(_excelDocument.RowsCount);
        _balances = new List<BcsReportBalanceModel>(_excelDocument.RowsCount);
        _exchangeRates = new List<BcsReportExchangeRateModel>(_excelDocument.RowsCount);
        _transactions = new List<BcsReportTransactionModel>(_excelDocument.RowsCount);
        _stockMoves = new List<BcsReportStockMoveModel>(_excelDocument.RowsCount);
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

        string? cellValue;

        var firstBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[0], StringComparison.OrdinalIgnoreCase) > -1);
        if (firstBlock is not null)
        {
            _rowId = fileStructure[firstBlock];

            var border = fileStructure.Skip(1).First().Key;

            var rowNo = _rowId + 1;

            while (!_excelDocument.TryGetCellValue(rowNo++, 1, border, out cellValue))
                if (!string.IsNullOrWhiteSpace(cellValue))
                    switch (cellValue)
                    {
                        case "USD":
                            GetAction("USD", Currencies.Usd);
                            break;
                        case "�����":
                            GetAction("�����", Currencies.Rub);
                            break;
                    }

            void GetAction(string value, Currencies? currency)
            {
                while (!_excelDocument.TryGetCellValue(_rowId++, 1, new[] { $"����� �� ������ {value}:", border }, out _))
                {
                    cellValue = _excelDocument.GetCellValue(_rowId, 2);

                    if (!string.IsNullOrWhiteSpace(cellValue) && _reportPatterns.ContainsKey(cellValue))
                        _reportPatterns[cellValue].Action(cellValue, currency);
                }
            }
        }

        var secondBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[2], StringComparison.OrdinalIgnoreCase) > -1);
        if (secondBlock is not null)
        {
            _rowId = fileStructure[secondBlock] + 3;

            while (!_excelDocument.TryGetCellValue(_rowId++, 1, "����� �� ������ �����:", out cellValue))
                if (!string.IsNullOrWhiteSpace(cellValue) && !_reportPatterns.ContainsKey(cellValue))
                    throw new PortfolioCoreException(Initiator, "�������� ������� ������ ������", "������ ������ ���������. ������ �� �������.");
        }

        var thirdBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[3], StringComparison.OrdinalIgnoreCase) > -1);
        if (thirdBlock is not null)
        {
            _rowId = fileStructure[thirdBlock];

            var borders = fileStructure.Keys
                .Where(x =>
                    BcsReportStructure.Points[4].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1
                    || BcsReportStructure.Points[5].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1)
                .ToArray();

            while (!_excelDocument.TryGetCellValue(_rowId++, 1, borders, out _))
            {
                cellValue = _excelDocument.GetCellValue(_rowId, 6);

                if (!string.IsNullOrWhiteSpace(cellValue) && _reportPatterns.ContainsKey(cellValue))
                    _reportPatterns[cellValue].Action(cellValue, null);
            }
        }

        while (!_excelDocument.TryGetCellValue(_rowId++, 1, "���� ����������� ������:", out _))
        {
            cellValue = _excelDocument.GetCellValue(_rowId, 12);

            if (!string.IsNullOrWhiteSpace(cellValue) && _reportPatterns.ContainsKey(cellValue))
                _reportPatterns[cellValue].Action(_excelDocument.GetCellValue(_rowId, 1)!, null);
        }

        model.Dividends = _dividends;
        model.Balances = _balances;
        model.Comissions = _comissions;
        model.ExchangeRates = _exchangeRates;
        model.Transactions = _transactions;
        model.StockMoves = _stockMoves;

        return model;
    }

    private void ParseDividend(string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new PortfolioCoreException(Initiator, DividendAction, "�� ������� ���������� ������");

        var currencyValue = currency.Value.ToString();
        var exchange = GetExchangeName(_rowId);

        var info = _excelDocument.GetCellValue(_rowId, 14);

        if (info is null)
            throw new PortfolioCoreException(Initiator, DividendAction, "�� ������� �������� ����������");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(Initiator, DividendAction, "�� ������� �������� ����");

        var dividendValue = _excelDocument.GetCellValue(_rowId, 6);

        if (dividendValue is null)
            throw new PortfolioCoreException(Initiator, DividendAction, "�� ������� �������� �����");

        _dividends.Add(new()
        {
            Info = info,
            Date = date,
            Exchange = exchange,
            Value = dividendValue,
            Currency = currencyValue,
            EventType = EventTypes.Dividend
        });

        var taxPosition = info.IndexOf("�����", StringComparison.OrdinalIgnoreCase);

        if (taxPosition <= -1)
            return;

        var taxValue = info[taxPosition..].Split(' ')[1];
        taxValue = taxValue.IndexOf('$') > -1 ? taxValue[1..] : taxValue;

        _comissions.Add(new()
        {
            Info = info,
            Date = date,
            Exchange = exchange,
            Value = taxValue,
            Currency = currencyValue,
            EventType = EventTypes.TaxIncome
        });
    }
    private void ParseComission(string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new PortfolioCoreException(Initiator, ComissionAction, "�� ������� ���������� ������");

        var currencyValue = currency.Value.ToString();
        var exchange = GetExchangeName(_rowId);

        var comissionValue = _excelDocument.GetCellValue(_rowId, 7);

        if (comissionValue is null)
            throw new PortfolioCoreException(Initiator, ComissionAction, "�� ������� �������� �����");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(Initiator, ComissionAction, "�� ������� �������� ����");

        _comissions.Add(new()
        {
            Info = value,
            Date = date,
            Exchange = exchange,
            Value = comissionValue,
            Currency = currencyValue,
            EventType = _reportPatterns[value].EventType
        });
    }
    private void ParseBalance(string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new PortfolioCoreException(Initiator, BalanceAction, "�� ������� ���������� ������");

        var currencyValue = currency.Value.ToString();
        var exchange = GetExchangeName(_rowId);

        var eventType = _reportPatterns[value].EventType;

        var columnNo = eventType switch
        {
            EventTypes.Increase => 6,
            EventTypes.Decrease => 7,
            EventTypes.InterestIncome => 6,
            EventTypes.Dividend => 6,
            _ => throw new PortfolioCoreException(Initiator, BalanceAction, "�� ������� ���������� ��� ��������")
        };

        var balanceValue = _excelDocument.GetCellValue(_rowId, columnNo);

        if (balanceValue is null)
            throw new PortfolioCoreException(Initiator, BalanceAction, "�� ������� �������� �����");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(Initiator, BalanceAction, "�� ������� �������� ����");

        _balances.Add(new()
        {
            Info = value,
            Date = date,
            Exchange = exchange,
            Value = balanceValue,
            Currency = currencyValue,
            EventType = _reportPatterns[value].EventType
        });
    }
    private void ParseExchangeRate(string value, Currencies? currency = null)
    {
        var currencyCode = _excelDocument.GetCellValue(_rowId, 1);

        if (currencyCode is null || !BcsReportStructure.ExchangeCurrencies.ContainsKey(currencyCode))
            throw new PortfolioCoreException(Initiator, ExchangeRateAction, "�� ������� ���������� ��� ������");

        var incomeCurrency = BcsReportStructure.ExchangeCurrencies[currencyCode].Income;
        var expenseCurrency = BcsReportStructure.ExchangeCurrencies[currencyCode].Expense;

        while (!_excelDocument.TryGetCellValue(_rowId++, 1, $"����� �� {currencyCode}:", out _))
        {
            var incomeValue = _excelDocument.GetCellValue(_rowId, 5);

            var date = _excelDocument.GetCellValue(_rowId, 1);

            if (date is null)
                throw new PortfolioCoreException(Initiator, ExchangeRateAction, "�� ������� �������� ����");

            var exchange = GetExchangeName(_rowId);

            if (!string.IsNullOrWhiteSpace(incomeValue))
            {
                var cost = _excelDocument.GetCellValue(_rowId, 4);

                if (cost is null)
                    throw new PortfolioCoreException(Initiator, ExchangeRateAction, "�� ������� �������� �����");

                _exchangeRates.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = incomeValue,
                    Cost = cost,
                    Currency = incomeCurrency,
                    EventType = EventTypes.Increase
                });
            }
            else
            {
                var expenseValue = _excelDocument.GetCellValue(_rowId, 8);

                if (expenseValue is null)
                    throw new PortfolioCoreException(Initiator, ExchangeRateAction, "�� ������� �������� ����������");

                var cost = _excelDocument.GetCellValue(_rowId, 7);

                if (cost is null)
                    throw new PortfolioCoreException(Initiator, ExchangeRateAction, "�� ������� �������� �����");

                _exchangeRates.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = expenseValue,
                    Cost = cost,
                    Currency = expenseCurrency,
                    EventType = EventTypes.Decrease
                });
            }
        }
    }
    private void ParseStockTransactions(string value, Currencies? currency = null)
    {
        var isin = _excel.GetCellValue(_rowId, 7);

        if (isin is null)
            throw new ApplicationException(nameof(ParseStockTransactions) + ".Isin not found");

        var infoArray = isin.Split(',').Select(x => x.Trim());

        var derivativeId = new DerivativeId(_derivatives.Keys.Intersect(infoArray).FirstOrDefault());
        var derivativeCode = new DerivativeCode(_derivatives[derivativeId.AsString][0]);

        var name = _excel.GetCellValue(_rowId, 1);

        while (!_excel.TryGetCellValue(_rowId++, 1, $"����� �� {name}:", out _))
        {
            var cellBuyValue = _excel.GetCellValue(_rowId, 4);

            var date = DateOnly.Parse(_excel.GetCellValue(_rowId, 1)!, Culture);
            currency = _excel.GetCellValue(_rowId, 10) switch
            {
                "USD" => Currencies.Usd,
                "�����" => Currencies.Rub,
                _ => throw new ArgumentOutOfRangeException(nameof(ParseStockTransactions) + $".Currency {currency} not found")
            };

            var exchange = _excel.GetCellValue(_rowId, 17);
            var exchangeId = !string.IsNullOrWhiteSpace(exchange) && BcsReportStructure.ExchangeTypes.ContainsKey(exchange)
                ? new ExchangeId(BcsReportStructure.ExchangeTypes[exchange])
                : throw new ApplicationException($"�� ������� ���������� �������� �� ��������: {exchange}");

            var dealId = new EntityStateId(Guid.NewGuid());
            IncomeModel incomeModel;
            ExpenseModel expenseModel;

            decimal dealValue;
            decimal dealCost;

            if (!string.IsNullOrWhiteSpace(cellBuyValue))
            {
                dealCost = decimal.Parse(_excel.GetCellValue(_rowId, 5)!);
                dealValue = decimal.Parse(cellBuyValue);

                incomeModel = new IncomeModel(dealId, derivativeId, derivativeCode, dealValue, date);

                var expenseDerivativeId = new DerivativeId(currency.Value.ToString());
                var expenseDerivativeCode = new DerivativeCode(_derivatives[expenseDerivativeId.AsString][0]);
                expenseModel = new ExpenseModel(dealId, expenseDerivativeId, expenseDerivativeCode, dealValue * dealCost, date);
            }
            else
            {
                dealValue = decimal.Parse(_excel.GetCellValue(_rowId, 7)!);
                dealCost = decimal.Parse(_excel.GetCellValue(_rowId, 8)!);

                var incomeDerivativeId = new DerivativeId(currency.Value.ToString());
                var incomeDerivativeCode = new DerivativeCode(_derivatives[incomeDerivativeId.AsString][0]);
                incomeModel = new IncomeModel(dealId, incomeDerivativeId, incomeDerivativeCode, dealValue * dealCost, date);

                expenseModel = new ExpenseModel(dealId, derivativeId, derivativeCode, dealValue, date);
            }

            var dealModel = new DealModel(dealId, incomeModel, expenseModel)
            {
                Date = date,
                Cost = dealCost,

                UserId = _userId,
                ProviderId = _providerId,
                AccountId = body.AccountId,
                ExchangeId = exchangeId,

                Info = name
            };

            body.AddDeal(dealModel);
        }
    }
    private void ParseAdditionalStockRelease(string value, Currencies? currency = null)
    {
        var ticker = value.Trim();

        var (derivative, derivativeCodes) = _derivatives.FirstOrDefault(x => x.Value.Contains(ticker, StringComparer.OrdinalIgnoreCase));

        var derivativeId = new DerivativeId(derivative);
        var derivativeCode = new DerivativeCode(derivativeCodes?.FirstOrDefault(x => x.Equals(ticker, StringComparison.OrdinalIgnoreCase)));

        body.AddEvent(new EventModel
        {
            DerivativeId = derivativeId,
            DerivativeCode = derivativeCode,

            Value = decimal.Parse(_excel.GetCellValue(_rowId, 7)!),

            EventTypeId = new EventTypeId(EventTypes.Increase),

            Date = DateOnly.Parse(_excel.GetCellValue(_rowId, 4)!, Culture),
            Info = _excel.GetCellValue(_rowId, 12),

            UserId = _userId,
            AccountId = body.AccountId,
            ProviderId = _providerId,
            ExchangeId = new ExchangeId(Exchanges.Spbex)
        });
    }

    private static ExcelDocument GetExcelDocument(byte[] payload)
    {
        try
        {
            var table = ExcelLoader.LoadTable(payload);
            return new ExcelDocument(table);
        }
        catch (Exception exception)
        {
            throw new PortfolioCoreException(Initiator, "�������������� ������ � excel ��������", exception.Message);
        }
    }
    private Dictionary<string, int> GetFileStructure(int rowId)
    {
        var structure = new Dictionary<string, int>(BcsReportStructure.Points.Length);

        while (!_excelDocument.TryGetCellValue(rowId++, 1, "���� ����������� ������:", out _))
            if (_excelDocument.TryGetCellValue(rowId, 1, BcsReportStructure.Points, out var cell))
                structure.Add(cell, rowId);

        return !structure.Any()
            ? throw new PortfolioCoreException(Initiator, "�������� ��������� ������ � ������", "��������� �� �������")
            : structure;
    }
    private (string DateStart, string DateEnd) GetReportPeriod(int rowId)
    {
        string? period = null;

        while (!_excelDocument.TryGetCellValue(rowId++, 1, "������:", out _))
            period = _excelDocument.GetCellValue(rowId, 5);

        if (string.IsNullOrWhiteSpace(period))
            throw new PortfolioCoreException(Initiator, "����� ������ � ������� ������", "������ �� ������");

        try
        {
            var periods = period.Split('\u0020');
            return (periods[1], periods[3]);
        }
        catch (Exception exception)
        {
            throw new PortfolioCoreException(Initiator, "����� ������ � ������� ������", exception);
        }
    }
    private string GetReportAgreement(int rowId)
    {
        string? agreement = null;

        while (!_excelDocument.TryGetCellValue(rowId++, 1, "����������� ����������:", out _))
            agreement = _excelDocument.GetCellValue(rowId, 5);

        return string.IsNullOrWhiteSpace(agreement)
            ? throw new PortfolioCoreException(Initiator, "����� ������ � ������ ����������", "����� �� ������")
            : agreement;
    }
    private string GetExchangeName(int rowId)
    {
        var exchange = _excelDocument.GetCellValue(rowId, 12);
        if (string.IsNullOrWhiteSpace(exchange))
            exchange = _excelDocument.GetCellValue(rowId, 11);
        else if (string.IsNullOrWhiteSpace(exchange))
            exchange = _excelDocument.GetCellValue(rowId, 10);
        else if (string.IsNullOrWhiteSpace(exchange))
            exchange = _excelDocument.GetCellValue(rowId, 14);

        return string.IsNullOrWhiteSpace(exchange) || !BcsReportStructure.ExchangeTypes.ContainsKey(exchange)
            ? throw new PortfolioCoreException(Initiator, "��������� ����� ��������", "��� �� �������")
            : BcsReportStructure.ExchangeTypes[exchange].ToString().ToUpper();
    }
}
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

public sealed class BcsReportDividendModel
{
    public string Info { get; set; } = null!;
    public string Date { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public EventTypes EventType { get; set; }
}
public sealed class BcsReportComissionModel
{
    public string Info { get; set; } = null!;
    public string Date { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public EventTypes EventType { get; set; }
}
public sealed class BcsReportBalanceModel
{
    public string Info { get; set; } = null!;
    public string Date { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public EventTypes EventType { get; set; }
}
public sealed class BcsReportExchangeRateModel
{
    public string Info { get; set; } = null!;
    public string Date { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Cost { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public EventTypes EventType { get; set; }
}
public sealed class BcsReportTransactionModel
{

}
public sealed class BcsReportStockMoveModel
{

}