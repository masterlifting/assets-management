using AM.Services.Portfolio.Core.Domain.Persistense.Models;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Deserialization.Reports.Bcs.Models;

using Shared.Data.Excel;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Deserialization.Reports.Bcs;

public sealed class BcsReport
{
    private const string Initiator = "������� ��� ������ (excel)";

    private readonly ExcelDocument _excelDocument;
    private int _rowId;

    private const string DividendsAction = "����� ����������";
    private const string ComissionsAction = "����� ��������";
    private const string BalanceAction = "����� �������� �������� ������� �� �������";
    private const string ExchangeRatesAction = "����� ������ �����";
    private const string TransactionsAction = "����� ����������";
    private const string StockMoveAction = "����� ����������� �����";

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
            { "ISIN:", (ParseTransactions, EventTypes.Default) },
            { "������. ������:", (ParseExchangeRate, EventTypes.Default) },
            { "�������������� �������� (����)", (ParseComission, EventTypes.TaxProvider) },
            { "�������� �� ����� \"�������� ��\"", (ParseComission, EventTypes.TaxProvider) },
            { "�������������� �������� (����)", (ParseComission, EventTypes.TaxProvider) },
            { "�������� �������� ����", (ParseComission, EventTypes.TaxProvider) },
            { "������ �� ����� �������� �������", (ParseComission, EventTypes.TaxProvider) },
            { "���. ������ ����� ", (ParseStockMove, EventTypes.Increase) },
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
                if (cellValue is not null)
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

                    if (cellValue is not null && _reportPatterns.ContainsKey(cellValue))
                        _reportPatterns[cellValue].Action(cellValue, currency);
                }
            }
        }

        var secondBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[2], StringComparison.OrdinalIgnoreCase) > -1);
        if (secondBlock is not null)
        {
            _rowId = fileStructure[secondBlock] + 3;

            while (!_excelDocument.TryGetCellValue(_rowId++, 1, "����� �� ������ �����:", out cellValue))
                if (cellValue is not null && !_reportPatterns.ContainsKey(cellValue))
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

                if (cellValue is not null && _reportPatterns.ContainsKey(cellValue))
                    _reportPatterns[cellValue].Action(cellValue, null);
            }
        }

        while (!_excelDocument.TryGetCellValue(_rowId++, 1, "���� ����������� ������:", out _))
        {
            cellValue = _excelDocument.GetCellValue(_rowId, 12);

            if (cellValue is not null && _reportPatterns.ContainsKey(cellValue))
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
            throw new PortfolioCoreException(Initiator, DividendsAction, "�� ������� ���������� ������");

        var currencyValue = currency.Value.ToString();
        var exchange = GetExchangeName(_rowId);

        var info = _excelDocument.GetCellValue(_rowId, 14);

        if (info is null)
            throw new PortfolioCoreException(Initiator, DividendsAction, "�� ������� �������� ����������");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(Initiator, DividendsAction, "�� ������� �������� ����");

        var sum = _excelDocument.GetCellValue(_rowId, 6);

        if (sum is null)
            throw new PortfolioCoreException(Initiator, DividendsAction, "�� ������� �������� �����");

        _dividends.Add(new()
        {
            Info = info,
            Date = date,
            Exchange = exchange,
            Sum = sum,
            Currency = currencyValue,
            EventType = "������� ���������"
        });

        var taxPosition = info.IndexOf("�����", StringComparison.OrdinalIgnoreCase);

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
            EventType = "�������� �� ������� ���������"
        });
    }
    private void ParseComission(string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new PortfolioCoreException(Initiator, ComissionsAction, "�� ������� ���������� ������");

        var currencyName = currency.Value.ToString();
        var exchange = GetExchangeName(_rowId);

        var sum = _excelDocument.GetCellValue(_rowId, 7);

        if (sum is null)
            throw new PortfolioCoreException(Initiator, ComissionsAction, "�� ������� �������� �����");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(Initiator, ComissionsAction, "�� ������� �������� ����");

        _comissions.Add(new()
        {
            Date = date,
            Exchange = exchange,
            Sum = sum,
            Currency = currencyName,
            EventType = value
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

        var sum = _excelDocument.GetCellValue(_rowId, columnNo);

        if (sum is null)
            throw new PortfolioCoreException(Initiator, BalanceAction, "�� ������� �������� �����");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(Initiator, BalanceAction, "�� ������� �������� ����");

        _balances.Add(new()
        {
            Date = date,
            Exchange = exchange,
            Sum = sum,
            Currency = currencyValue,
            EventType = value
        });
    }
    private void ParseExchangeRate(string value, Currencies? currency = null)
    {
        var currencyCode = _excelDocument.GetCellValue(_rowId, 1);

        if (currencyCode is null || !BcsReportStructure.ExchangeCurrencies.ContainsKey(currencyCode))
            throw new PortfolioCoreException(Initiator, ExchangeRatesAction, "�� ������� ���������� ��� ������");

        var incomeCurrency = BcsReportStructure.ExchangeCurrencies[currencyCode].Income;
        var expenseCurrency = BcsReportStructure.ExchangeCurrencies[currencyCode].Expense;

        while (!_excelDocument.TryGetCellValue(_rowId++, 1, $"����� �� {currencyCode}:", out _))
        {
            var incomeValue = _excelDocument.GetCellValue(_rowId, 5);

            var date = _excelDocument.GetCellValue(_rowId, 1);

            if (date is null)
                throw new PortfolioCoreException(Initiator, ExchangeRatesAction, "�� ������� �������� ����");

            var exchange = GetExchangeName(_rowId, 14);

            if (incomeValue is not null)
            {
                var sum = _excelDocument.GetCellValue(_rowId, 4);

                if (sum is null)
                    throw new PortfolioCoreException(Initiator, ExchangeRatesAction, "�� ������� �������� �����");

                _exchangeRates.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = incomeValue,
                    Sum = sum,
                    Currency = incomeCurrency,
                    EventType = "������� ������"
                });
            }
            else
            {
                var expenseValue = _excelDocument.GetCellValue(_rowId, 8);

                if (expenseValue is null)
                    throw new PortfolioCoreException(Initiator, ExchangeRatesAction, "�� ������� �������� ���������� �������");

                var sum = _excelDocument.GetCellValue(_rowId, 7);

                if (sum is null)
                    throw new PortfolioCoreException(Initiator, ExchangeRatesAction, "�� ������� �������� �����");

                _exchangeRates.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = expenseValue,
                    Sum = sum,
                    Currency = expenseCurrency,
                    EventType = "������� ������"
                });
            }
        }
    }
    private void ParseTransactions(string value, Currencies? currency = null)
    {
        var isin = _excelDocument.GetCellValue(_rowId, 7);

        if (isin is null)
            throw new PortfolioCoreException(Initiator, TransactionsAction, "�� ������� ���������� ISIN");

        var name = _excelDocument.GetCellValue(_rowId, 1);

        while (!_excelDocument.TryGetCellValue(_rowId++, 1, $"����� �� {name}:", out _))
        {
            var incomeValue = _excelDocument.GetCellValue(_rowId, 4);

            var date = _excelDocument.GetCellValue(_rowId, 1);

            if (date is null)
                throw new PortfolioCoreException(Initiator, TransactionsAction, "�� ������� �������� ����");

            var currencyCode = _excelDocument.GetCellValue(_rowId, 10);

            if (currencyCode is null)
                throw new PortfolioCoreException(Initiator, TransactionsAction, "�� ������� ���������� ������");

            var currencyName = currencyCode switch
            {
                "USD" => Currencies.Usd.ToString(),
                "�����" => Currencies.Rub.ToString(),
                _ => throw new PortfolioCoreException(Initiator, TransactionsAction, "�� ������� �������� ������")
            };

            var exchange = GetExchangeName(_rowId, 17);

            if (incomeValue is not null)
            {
                var sum = _excelDocument.GetCellValue(_rowId, 5);

                if (sum is null)
                    throw new PortfolioCoreException(Initiator, TransactionsAction, "�� ������� �������� �����");

                _transactions.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = incomeValue,
                    Sum = sum,
                    Currency = currencyName,
                    EventType = "������� �����",
                    Info = isin
                });
            }
            else
            {
                var expenseValue = _excelDocument.GetCellValue(_rowId, 7);

                if (expenseValue is null)
                    throw new PortfolioCoreException(Initiator, TransactionsAction, "�� ������� �������� ���������� �������");

                var sum = _excelDocument.GetCellValue(_rowId, 8);

                if (sum is null)
                    throw new PortfolioCoreException(Initiator, TransactionsAction, "�� ������� �������� �����");

                _transactions.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = expenseValue,
                    Sum = sum,
                    Currency = currencyName,
                    EventType = "������� �����",
                    Info = isin
                });
            }
        }
    }
    private void ParseStockMove(string value, Currencies? currency = null)
    {
        var date = _excelDocument.GetCellValue(_rowId, 4);

        if (date is null)
            throw new PortfolioCoreException(Initiator, StockMoveAction, "�� ������� �������� ����");

        var moveValue = _excelDocument.GetCellValue(_rowId, 7);

        if (moveValue is null)
            throw new PortfolioCoreException(Initiator, StockMoveAction, "�� ������� �������� ����������");

        var info = _excelDocument.GetCellValue(_rowId, 12);

        if (info is null)
            throw new PortfolioCoreException(Initiator, StockMoveAction, "�� ������� �������� ����������");

        _stockMoves.Add(new()
        {
            Ticker = value.Trim(),
            Date = date,
            Value = moveValue,
            Info = info
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
            if (_excelDocument.TryGetCellValue(rowId, 1, BcsReportStructure.Points, out var cellValue))
                if(cellValue is not null)
                    structure.Add(cellValue, rowId);

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
    private string GetExchangeName(int rowId, int? columnId = null)
    {
        string? exchange;

        if (columnId.HasValue)
            exchange = _excelDocument.GetCellValue(rowId, columnId.Value);
        else
        {
            exchange = _excelDocument.GetCellValue(rowId, 12);

            if (exchange is null)
                exchange = _excelDocument.GetCellValue(rowId, 10);
            else exchange ??= _excelDocument.GetCellValue(rowId, 11);
        }

        return exchange is not null && BcsReportStructure.ExchangeTypes.ContainsKey(exchange)
            ? BcsReportStructure.ExchangeTypes[exchange].ToString().ToUpper()
            : throw new PortfolioCoreException(Initiator, "��������� ����� ��������", "��� �� �������");
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
    public string Sum { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public string EventType { get; set; } = null!;
}
public sealed class BcsReportComissionModel
{
    public string Date { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string Sum { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public string EventType { get; set; } = null!;
}
public sealed class BcsReportBalanceModel
{
    public string Date { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string Sum { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public string EventType { get; set; } = null!;
}
public sealed class BcsReportExchangeRateModel
{
    public string Date { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Sum { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public string EventType { get; set; } = null!;
}
public sealed class BcsReportTransactionModel
{
    public string Info { get; set; } = null!;
    public string Date { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Sum { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public string EventType { get; set; } = null!;
}
public sealed class BcsReportStockMoveModel
{
    public string Ticker { get; set; } = null!;
    public string Date { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Info { get; set; } = null!;
}