using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs.Models;
using Shared.Data.Excel;
using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs;

public sealed class BcsReportFileParser
{
    private string _initiator = "����������: ";

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

    public BcsReportFileParser(byte[] payload)
    {
        _excelDocument = GetExcelDocument(payload);

        _reportPatterns = new(StringComparer.OrdinalIgnoreCase)
        {
            { "9", (ParseDividend, EventTypes.Dividend) },
            { "14 6", (ParseComission, EventTypes.TaxProvider) },
            { "14 8", (ParseComission, EventTypes.TaxProvider) },
            { "14 2 12 5 4", (ParseComission, EventTypes.TaxDepositary) },
            { "8 2", (ParseComission, EventTypes.TaxDepositary) },
            { "4", (ParseComission, EventTypes.TaxCountry) },
            { "6 2", (ParseBalance, EventTypes.Increase) },
            { "5 2", (ParseBalance, EventTypes.Decrease) },
            { "ISIN:", (ParseTransactions, EventTypes.Default) },
            { "6. 6:", (ParseExchangeRate, EventTypes.Default) },
            { "14 8 (4) ", (ParseComission, EventTypes.TaxProvider) },
            { "8 2 5 \"8 2\"", (ParseComission, EventTypes.TaxProvider) },
            { "14 8 (4)", (ParseComission, EventTypes.TaxProvider) },
            { "8 8 4", (ParseComission, EventTypes.TaxProvider) },
            { "6 2 5 8 7", (ParseComission, EventTypes.TaxProvider) },
            { "3. 6 5 ", (ParseStockMove, EventTypes.Increase) },
            { "8 2 6 \"8 2\"", (ParseBalance, EventTypes.InterestProfit) },
            { "8 2 6 \"8\"", (ParseBalance, EventTypes.InterestProfit) },
            { "13 (4*)", (ParseComission, EventTypes.TaxDepositary) },
            { "10 10 2 6", (ParseBalance, EventTypes.Dividend) }
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

        _initiator += model.Agreement;

        var (dateStart, dateEnd) = GetReportPeriod(_rowId);

        model.DateStart = dateStart;
        model.DateEnd = dateEnd;

        var fileStructure = GetFileStructure(0);

        string? cellValue;

        var firstBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points[0], StringComparison.OrdinalIgnoreCase) > -1);
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
                        case "Рубль":
                            GetAction("Рубль", Currencies.Rub);
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

        var secondBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points[2], StringComparison.OrdinalIgnoreCase) > -1);
        if (secondBlock is not null)
        {
            _rowId = fileStructure[secondBlock] + 3;

            while (!_excelDocument.TryGetCellValue(_rowId++, 1, "����� �� ������ �����:", out cellValue))
                if (cellValue is not null && !_reportPatterns.ContainsKey(cellValue))
                    throw new PortfolioCoreException(_initiator, "�������� ������� ������ ������", "������ ������ ���������. ������ �� �������.");
        }

        var thirdBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points[3], StringComparison.OrdinalIgnoreCase) > -1);
        if (thirdBlock is not null)
        {
            _rowId = fileStructure[thirdBlock];

            var borders = fileStructure.Keys
                .Where(x =>
                    BcsReportFileStructure.Points[4].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1
                    || BcsReportFileStructure.Points[5].IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1)
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
            throw new PortfolioCoreException(_initiator, DividendsAction, "�� ������� ���������� ������");

        var currencyValue = currency.Value.ToString();
        var exchange = GetExchangeName(DividendsAction, _rowId);

        var info = _excelDocument.GetCellValue(_rowId, 14);

        if (info is null)
            throw new PortfolioCoreException(_initiator, DividendsAction, "�� ������� �������� ����������");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, DividendsAction, "�� ������� �������� ����");

        var sum = _excelDocument.GetCellValue(_rowId, 6);

        if (sum is null)
            throw new PortfolioCoreException(_initiator, DividendsAction, "�� ������� �������� �����");

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
            throw new PortfolioCoreException(_initiator, ComissionsAction, "�� ������� ���������� ������");

        var currencyName = currency.Value.ToString();
        var exchange = GetExchangeName(ComissionsAction, _rowId);

        var sum = _excelDocument.GetCellValue(_rowId, 7);

        if (sum is null)
            throw new PortfolioCoreException(_initiator, ComissionsAction, "�� ������� �������� �����");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, ComissionsAction, "�� ������� �������� ����");

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
            throw new PortfolioCoreException(_initiator, BalanceAction, "�� ������� ���������� ������");

        var currencyValue = currency.Value.ToString();
        var exchange = GetExchangeName(BalanceAction, _rowId);

        var eventType = _reportPatterns[value].EventType;

        var columnNo = eventType switch
        {
            EventTypes.Increase => 6,
            EventTypes.Decrease => 7,
            EventTypes.InterestProfit => 6,
            EventTypes.Dividend => 6,
            _ => throw new PortfolioCoreException(_initiator, BalanceAction, "�� ������� ���������� ��� ��������")
        };

        var sum = _excelDocument.GetCellValue(_rowId, columnNo);

        if (sum is null)
            throw new PortfolioCoreException(_initiator, BalanceAction, "�� ������� �������� �����");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, BalanceAction, "�� ������� �������� ����");

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

        if (currencyCode is null || !BcsReportFileStructure.ExchangeCurrencies.ContainsKey(currencyCode))
            throw new PortfolioCoreException(_initiator, ExchangeRatesAction, "�� ������� ���������� ��� ������");

        var incomeCurrency = BcsReportFileStructure.ExchangeCurrencies[currencyCode].Income;
        var expenseCurrency = BcsReportFileStructure.ExchangeCurrencies[currencyCode].Expense;

        while (!_excelDocument.TryGetCellValue(++_rowId, 1, $"����� �� {currencyCode}:", out _))
        {
            var incomeValue = _excelDocument.GetCellValue(_rowId, 5);
            if (incomeValue is not null && decimal.TryParse(incomeValue, out _))
            {
                var exchange = GetExchangeName(ExchangeRatesAction, _rowId);

                var date = _excelDocument.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesAction, "�� ������� �������� ����");

                var sum = _excelDocument.GetCellValue(_rowId, 4);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesAction, "�� ������� �������� �����");

                _exchangeRates.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = incomeValue,
                    Sum = sum,
                    Currency = incomeCurrency,
                    EventType = "������� ������"
                });

                continue;
            }

            var expenseValue = _excelDocument.GetCellValue(_rowId, 8);
            if (expenseValue is not null && decimal.TryParse(expenseValue, out _))
            {
                var exchange = GetExchangeName(ExchangeRatesAction, _rowId);

                var date = _excelDocument.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesAction, "�� ������� �������� ����");

                var sum = _excelDocument.GetCellValue(_rowId, 7);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesAction, "�� ������� �������� �����");

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
            throw new PortfolioCoreException(_initiator, TransactionsAction, "�� ������� ���������� ISIN");

        var name = _excelDocument.GetCellValue(_rowId, 1);

        while (!_excelDocument.TryGetCellValue(++_rowId, 1, $"����� �� {name}:", out _))
        {
            var incomeValue = _excelDocument.GetCellValue(_rowId, 4);
            if (incomeValue is not null && decimal.TryParse(incomeValue, out _))
            {
                var exchange = GetExchangeName(TransactionsAction, _rowId);

                var date = _excelDocument.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "�� ������� �������� ����");

                var currencyCode = _excelDocument.GetCellValue(_rowId, 10);
                if (currencyCode is null)
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "�� ������� �������� ������");
                var currencyName = currencyCode switch
                {
                    "USD" => Currencies.Usd.ToString(),
                    "�����" => Currencies.Rub.ToString(),
                    _ => throw new PortfolioCoreException(_initiator, TransactionsAction, "�� ������� ���������� ������")
                };

                var sum = _excelDocument.GetCellValue(_rowId, 5);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "�� ������� �������� �����");

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

                continue;
            }

            var expenseValue = _excelDocument.GetCellValue(_rowId, 7);
            if (expenseValue is not null && decimal.TryParse(expenseValue, out _))
            {
                var exchange = GetExchangeName(TransactionsAction, _rowId);

                var date = _excelDocument.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "�� ������� �������� ����");

                var currencyCode = _excelDocument.GetCellValue(_rowId, 10);
                if (currencyCode is null)
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "�� ������� �������� ������");
                var currencyName = currencyCode switch
                {
                    "USD" => Currencies.Usd.ToString(),
                    "�����" => Currencies.Rub.ToString(),
                    _ => throw new PortfolioCoreException(_initiator, TransactionsAction, "�� ������� ���������� ������")
                };

                var sum = _excelDocument.GetCellValue(_rowId, 8);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "�� ������� �������� �����");

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
            throw new PortfolioCoreException(_initiator, StockMoveAction, "�� ������� �������� ����");

        var moveValue = _excelDocument.GetCellValue(_rowId, 7);

        if (moveValue is null)
            throw new PortfolioCoreException(_initiator, StockMoveAction, "�� ������� �������� ����������");

        var info = _excelDocument.GetCellValue(_rowId, 12);

        if (info is null)
            throw new PortfolioCoreException(_initiator, StockMoveAction, "�� ������� �������� ����������");

        _stockMoves.Add(new()
        {
            Ticker = value.Trim(),
            Date = date,
            Value = moveValue,
            Info = info
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
            throw new PortfolioCoreException(_initiator, "�������������� ������ � excel ��������", exception.Message);
        }
    }
    private Dictionary<string, int> GetFileStructure(int rowId)
    {
        var structure = new Dictionary<string, int>(BcsReportFileStructure.Points.Length);

        while (!_excelDocument.TryGetCellValue(rowId++, 1, "Дата составления отчета:", out _))
            if (_excelDocument.TryGetCellValue(rowId, 1, BcsReportFileStructure.Points, out var cellValue))
                if (cellValue is not null)
                    structure.Add(cellValue, rowId);

        return !structure.Any()
            ? throw new PortfolioCoreException(_initiator, "�������� ��������� ������ � ������", "��������� �� �������")
            : structure;
    }
    private (string DateStart, string DateEnd) GetReportPeriod(int rowId)
    {
        string? period = null;

        while (!_excelDocument.TryGetCellValue(rowId++, 1, "Период:", out _))
            period = _excelDocument.GetCellValue(rowId, 5);

        if (string.IsNullOrWhiteSpace(period))
            throw new PortfolioCoreException(_initiator, "����� ������ � ������� ������", "������ �� ������");

        try
        {
            var periods = period.Split('\u0020');
            return (periods[1], periods[3]);
        }
        catch (Exception exception)
        {
            throw new PortfolioCoreException(_initiator, "����� ������ � ������� ������", exception);
        }
    }
    private string GetReportAgreement(int rowId)
    {
        string? agreement = null;

        while (!_excelDocument.TryGetCellValue(rowId++, 1, "Генеральное соглашение:", out _))
            agreement = _excelDocument.GetCellValue(rowId, 5);

        return string.IsNullOrWhiteSpace(agreement)
            ? throw new PortfolioCoreException(_initiator, "����� ������ � ������ ����������", "����� �� ������")
            : agreement;
    }
    private string GetExchangeName(string actionName, int rowId)
    {
        for (var columnNo = 10; columnNo < 20; columnNo++)
        {
            var exchange = _excelDocument.GetCellValue(rowId, columnNo);
            if (exchange is not null && BcsReportFileStructure.ExchangeTypes.ContainsKey(exchange))
                return BcsReportFileStructure.ExchangeTypes[exchange].ToString().ToUpper();
        }

        throw new PortfolioCoreException(_initiator, actionName, $"��� �������� �� �������. ������: {rowId}");
    }
}