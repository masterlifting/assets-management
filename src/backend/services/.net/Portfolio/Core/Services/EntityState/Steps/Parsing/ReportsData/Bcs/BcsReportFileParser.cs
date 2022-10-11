using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs.Models;
using Shared.Data.Excel;
using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs;

public sealed class BcsReportFileParser
{
    private string _initiator = "Соглашение: ";

    private readonly ExcelDocument _excelDocument;
    private int _rowId;

    private const string DividendsAction = "Поиск дивидендов";
    private const string ComissionsAction = "Поиск комиссий";
    private const string BalanceAction = "Поиск движений денежных средств на балансе";
    private const string ExchangeRatesAction = "Поиск обмена валют";
    private const string TransactionsAction = "Поиск транзакций";
    private const string StockMoveAction = "Поиск перемещений акций";

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
            { "Дивиденды", (ParseDividend, EventTypes.Dividend) },
            { "Урегулирование сделок", (ParseComission, EventTypes.TaxProvider) },
            { "Вознаграждение компании", (ParseComission, EventTypes.TaxProvider) },
            { "Вознаграждение за обслуживание счета депо", (ParseComission, EventTypes.TaxDepositary) },
            { "Хранение ЦБ", (ParseComission, EventTypes.TaxDepositary) },
            { "НДФЛ", (ParseComission, EventTypes.Ndfl) },
            { "Приход ДС", (ParseBalance, EventTypes.Increase) },
            { "Вывод ДС", (ParseBalance, EventTypes.Decrease) },
            { "ISIN:", (ParseTransactions, EventTypes.Default) },
            { "Сопряж. валюта:", (ParseExchangeRate, EventTypes.Default) },
            { "Вознаграждение компании (СВОП)", (ParseComission, EventTypes.TaxProvider) },
            { "Комиссия за займы \"овернайт ЦБ\"", (ParseComission, EventTypes.TaxProvider) },
            { "Вознаграждение компании (репо)", (ParseComission, EventTypes.TaxProvider) },
            { "Комиссия Биржевой гуру", (ParseComission, EventTypes.TaxProvider) },
            { "Оплата за вывод денежных средств", (ParseComission, EventTypes.TaxProvider) },
            { "Доп. выпуск акций ", (ParseStockMove, EventTypes.Increase) },
            { "Проценты по займам \"овернайт ЦБ\"", (ParseBalance, EventTypes.InterestIncome) },
            { "Проценты по займам \"овернайт\"", (ParseBalance, EventTypes.InterestIncome) },
            { "Распределение (4*)", (ParseComission, EventTypes.TaxDepositary) },
            { "Возмещение дивидендов по сделке", (ParseBalance, EventTypes.Dividend) }
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
                while (!_excelDocument.TryGetCellValue(_rowId++, 1, new[] { $"Итого по валюте {value}:", border }, out _))
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

            while (!_excelDocument.TryGetCellValue(_rowId++, 1, "Итого по валюте Рубль:", out cellValue))
                if (cellValue is not null && !_reportPatterns.ContainsKey(cellValue))
                    throw new PortfolioCoreException(_initiator, "Проверка наличия данных отчета", "Чтение отчета завершено. Данных не найдено.");
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

        while (!_excelDocument.TryGetCellValue(_rowId++, 1, "Дата составления отчета:", out _))
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
            throw new PortfolioCoreException(_initiator, DividendsAction, "Не удалось определить валюту");

        var currencyValue = currency.Value.ToString();
        var exchange = GetExchangeName(DividendsAction, _rowId);

        var info = _excelDocument.GetCellValue(_rowId, 14);

        if (info is null)
            throw new PortfolioCoreException(_initiator, DividendsAction, "Не удалось получить информацию");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, DividendsAction, "Не удалось получить дату");

        var sum = _excelDocument.GetCellValue(_rowId, 6);

        if (sum is null)
            throw new PortfolioCoreException(_initiator, DividendsAction, "Не удалось получить сумму");

        _dividends.Add(new()
        {
            Info = info,
            Date = date,
            Exchange = exchange,
            Sum = sum,
            Currency = currencyValue,
            EventType = "Выплата дивиденда"
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
            EventType = "Комиссия по выплате дивиденда"
        });
    }
    private void ParseComission(string value, Currencies? currency)
    {
        if (!currency.HasValue)
            throw new PortfolioCoreException(_initiator, ComissionsAction, "Не удалось определить валюту");

        var currencyName = currency.Value.ToString();
        var exchange = GetExchangeName(ComissionsAction, _rowId);

        var sum = _excelDocument.GetCellValue(_rowId, 7);

        if (sum is null)
            throw new PortfolioCoreException(_initiator, ComissionsAction, "Не удалось получить сумму");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, ComissionsAction, "Не удалось получить дату");

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
            throw new PortfolioCoreException(_initiator, BalanceAction, "Не удалось определить валюту");

        var currencyValue = currency.Value.ToString();
        var exchange = GetExchangeName(BalanceAction, _rowId);

        var eventType = _reportPatterns[value].EventType;

        var columnNo = eventType switch
        {
            EventTypes.Increase => 6,
            EventTypes.Decrease => 7,
            EventTypes.InterestIncome => 6,
            EventTypes.Dividend => 6,
            _ => throw new PortfolioCoreException(_initiator, BalanceAction, "Не удалось определить тип операции")
        };

        var sum = _excelDocument.GetCellValue(_rowId, columnNo);

        if (sum is null)
            throw new PortfolioCoreException(_initiator, BalanceAction, "Не удалось получить сумму");

        var date = _excelDocument.GetCellValue(_rowId, 1);

        if (date is null)
            throw new PortfolioCoreException(_initiator, BalanceAction, "Не удалось получить дату");

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
            throw new PortfolioCoreException(_initiator, ExchangeRatesAction, "Не удалось определить код валюты");

        var incomeCurrency = BcsReportFileStructure.ExchangeCurrencies[currencyCode].Income;
        var expenseCurrency = BcsReportFileStructure.ExchangeCurrencies[currencyCode].Expense;

        while (!_excelDocument.TryGetCellValue(++_rowId, 1, $"Итого по {currencyCode}:", out _))
        {
            var incomeValue = _excelDocument.GetCellValue(_rowId, 5);
            if (incomeValue is not null && decimal.TryParse(incomeValue, out _))
            {
                var exchange = GetExchangeName(ExchangeRatesAction, _rowId);

                var date = _excelDocument.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesAction, "Не удалось получить дату");

                var sum = _excelDocument.GetCellValue(_rowId, 4);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesAction, "Не удалось получить сумму");

                _exchangeRates.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = incomeValue,
                    Sum = sum,
                    Currency = incomeCurrency,
                    EventType = "Покупка валюты"
                });

                continue;
            }

            var expenseValue = _excelDocument.GetCellValue(_rowId, 8);
            if (expenseValue is not null && decimal.TryParse(expenseValue, out _))
            {
                var exchange = GetExchangeName(ExchangeRatesAction, _rowId);

                var date = _excelDocument.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesAction, "Не удалось получить дату");

                var sum = _excelDocument.GetCellValue(_rowId, 7);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, ExchangeRatesAction, "Не удалось получить сумму");

                _exchangeRates.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = expenseValue,
                    Sum = sum,
                    Currency = expenseCurrency,
                    EventType = "Продажа валюты"
                });
            }
        }
    }
    private void ParseTransactions(string value, Currencies? currency = null)
    {
        var isin = _excelDocument.GetCellValue(_rowId, 7);

        if (isin is null)
            throw new PortfolioCoreException(_initiator, TransactionsAction, "Не удалось определить ISIN");

        var name = _excelDocument.GetCellValue(_rowId, 1);

        while (!_excelDocument.TryGetCellValue(++_rowId, 1, $"Итого по {name}:", out _))
        {
            var incomeValue = _excelDocument.GetCellValue(_rowId, 4);
            if (incomeValue is not null && decimal.TryParse(incomeValue, out _))
            {
                var exchange = GetExchangeName(TransactionsAction, _rowId);

                var date = _excelDocument.GetCellValue(_rowId, 1);
                if (date is null)
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "Не удалось получить дату");

                var currencyCode = _excelDocument.GetCellValue(_rowId, 10);
                if (currencyCode is null)
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "Не удалось получить валюту");
                var currencyName = currencyCode switch
                {
                    "USD" => Currencies.Usd.ToString(),
                    "Рубль" => Currencies.Rub.ToString(),
                    _ => throw new PortfolioCoreException(_initiator, TransactionsAction, "Не удалось определить валюту")
                };

                var sum = _excelDocument.GetCellValue(_rowId, 5);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "Не удалось получить сумму");

                _transactions.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = incomeValue,
                    Sum = sum,
                    Currency = currencyName,
                    EventType = "Продажа акции",
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
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "Не удалось получить дату");

                var currencyCode = _excelDocument.GetCellValue(_rowId, 10);
                if (currencyCode is null)
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "Не удалось получить валюту");
                var currencyName = currencyCode switch
                {
                    "USD" => Currencies.Usd.ToString(),
                    "Рубль" => Currencies.Rub.ToString(),
                    _ => throw new PortfolioCoreException(_initiator, TransactionsAction, "Не удалось определить валюту")
                };

                var sum = _excelDocument.GetCellValue(_rowId, 8);
                if (sum is null)
                    throw new PortfolioCoreException(_initiator, TransactionsAction, "Не удалось получить сумму");

                _transactions.Add(new()
                {
                    Date = date,
                    Exchange = exchange,
                    Value = expenseValue,
                    Sum = sum,
                    Currency = currencyName,
                    EventType = "Покупка акции",
                    Info = isin
                });
            }
        }
    }
    private void ParseStockMove(string value, Currencies? currency = null)
    {
        var date = _excelDocument.GetCellValue(_rowId, 4);

        if (date is null)
            throw new PortfolioCoreException(_initiator, StockMoveAction, "Не удалось получить дату");

        var moveValue = _excelDocument.GetCellValue(_rowId, 7);

        if (moveValue is null)
            throw new PortfolioCoreException(_initiator, StockMoveAction, "Не удалось получить количество");

        var info = _excelDocument.GetCellValue(_rowId, 12);

        if (info is null)
            throw new PortfolioCoreException(_initiator, StockMoveAction, "Не удалось получить информацию");

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
            throw new PortfolioCoreException(_initiator, "Преобразование данных в excel документ", exception.Message);
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
            ? throw new PortfolioCoreException(_initiator, "Загрузка структуры отчета в память", "Структура не найдена")
            : structure;
    }
    private (string DateStart, string DateEnd) GetReportPeriod(int rowId)
    {
        string? period = null;

        while (!_excelDocument.TryGetCellValue(rowId++, 1, "Период:", out _))
            period = _excelDocument.GetCellValue(rowId, 5);

        if (string.IsNullOrWhiteSpace(period))
            throw new PortfolioCoreException(_initiator, "Поиск данных о периоде отчета", "Период не найден");

        try
        {
            var periods = period.Split('\u0020');
            return (periods[1], periods[3]);
        }
        catch (Exception exception)
        {
            throw new PortfolioCoreException(_initiator, "Поиск данных о периоде отчета", exception);
        }
    }
    private string GetReportAgreement(int rowId)
    {
        string? agreement = null;

        while (!_excelDocument.TryGetCellValue(rowId++, 1, "Генеральное соглашение:", out _))
            agreement = _excelDocument.GetCellValue(rowId, 5);

        return string.IsNullOrWhiteSpace(agreement)
            ? throw new PortfolioCoreException(_initiator, "Поиск данных о номере соглашения", "Номер не найден")
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

        throw new PortfolioCoreException(_initiator, actionName, $"Имя площадки не найдено. Строка: {rowId}");
    }
}