using System.Globalization;

using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Domain.Persistense.Models;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Serialization.Reports.Bcs.Models;

using Microsoft.Extensions.Logging;

using Shared.Data.Excel;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Serialization.Reports.Bcs
{
    public sealed class BcsReport
    {
        public ReportData File { get; }
        public BcsReportHeader? Header { get; set; }
        public BcsReportBody? Body { get; set; }

        private readonly ProviderId _providerId = new(Providers.Bcs);
        private readonly UserId _userId;

        private readonly IFormatProvider _culture;
        private readonly ILogger _logger;

        private int _rowId;
        private readonly ExcelDocument _excel;

        private readonly IDictionary<string, string[]> _derivatives;
        private readonly Dictionary<string, int> _reportPoints;
        private readonly Dictionary<string, (Action<BcsReportBody, string, Currencies?> Action, EventTypes EventType)> _reportPatterns;

        public BcsReport(ILogger logger, ReportData file, IDictionary<string, string[]> derivatives)
        {
            File = file;

            try
            {
                var table = ExcelLoader.LoadTable(file.Payload);
                _excel = new ExcelDocument(table);
            }
            catch (Exception exception)
            {
                throw new PortfolioCoreException("разбор отчета БКС", "получение excel таблицы отчета", exception.Message);
            }

            _logger = logger;
            _derivatives = derivatives;
            _userId = new UserId(file.UserId);
            _culture = new CultureInfo("ru-RU");
            _reportPoints = new Dictionary<string, int>(BcsReportStructure.Points.Length);
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
            while (!_excel.TryGetCellValue(_rowId++, 1, "Дата составления отчета:", out _))
            {
                if (_excel.TryGetCellValue(_rowId, 1, BcsReportStructure.Points, out var cell))
                    _reportPoints.Add(cell, _rowId);
            }

            if (!_reportPoints.Any())
                throw new ApplicationException("Report structure not recognized");

            string? period = null;
            while (!_excel.TryGetCellValue(_rowId++, 1, "Период:", out _))
                period = _excel.GetCellValue(_rowId, 5);

            if (period is null)
                throw new ApplicationException($"Agreement period '{period}' not recognized");
            var dates = period.Split(' ');

            string? agreement = null;
            while (!_excel.TryGetCellValue(_rowId++, 1, "Генеральное соглашение:", out _))
                agreement = _excel.GetCellValue(_rowId, 5);

            return new BcsReportHeader
            {
                Agreement = agreement ?? throw new ApplicationException($"Agreement '{agreement}' not found"),
                DateStart = DateOnly.Parse(dates[1], _culture),
                DateEnd = DateOnly.Parse(dates[3], _culture)
            };
        }
        public BcsReportBody GetBody(int accountId)
        {
            var body = new BcsReportBody(accountId, _excel.RowsCount);

            string? cellValue;

            var firstBlock = _reportPoints.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[0], StringComparison.OrdinalIgnoreCase) > -1);
            if (firstBlock is not null)
            {
                _rowId = _reportPoints[firstBlock];

                var border = _reportPoints.Skip(1).First().Key;

                var rowNo = _rowId++;
                while (!_excel.TryGetCellValue(rowNo++, 1, border, out cellValue))
                    if (!string.IsNullOrWhiteSpace(cellValue))
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
                    while (!_excel.TryGetCellValue(_rowId++, 1, new[] { $"Итого по валюте {value}:", border }, out _))
                    {
                        cellValue = _excel.GetCellValue(_rowId, 2);

                        if (string.IsNullOrWhiteSpace(cellValue))
                            continue;

                        if (_reportPatterns.ContainsKey(cellValue))
                            _reportPatterns[cellValue].Action(body, cellValue, currency);
                        else if (!BcsReportStructure.SkippedActions.Contains(cellValue, StringComparer.OrdinalIgnoreCase))
                            _logger.LogWarning($"parse {firstBlock}", cellValue, "not recognized");
                    }
                }
            }

            var secondBlock = _reportPoints.Keys.FirstOrDefault(x => x.IndexOf(BcsReportStructure.Points[2], StringComparison.OrdinalIgnoreCase) > -1);
            if (secondBlock is not null)
            {
                _rowId = _reportPoints[secondBlock] + 3;

                while (!_excel.TryGetCellValue(_rowId++, 1, "Итого по валюте Рубль:", out cellValue))
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

                while (!_excel.TryGetCellValue(_rowId++, 1, borders, out _))
                {
                    cellValue = _excel.GetCellValue(_rowId, 6);

                    if (!string.IsNullOrWhiteSpace(cellValue) && _reportPatterns.ContainsKey(cellValue))
                        _reportPatterns[cellValue].Action(body, cellValue, null);
                }
            }

            while (!_excel.TryGetCellValue(_rowId++, 1, "Дата составления отчета:", out _))
            {
                cellValue = _excel.GetCellValue(_rowId, 12);

                if (!string.IsNullOrWhiteSpace(cellValue) && _reportPatterns.ContainsKey(cellValue))
                    _reportPatterns[cellValue].Action(body, _excel.GetCellValue(_rowId, 1)!, null);
            }

            return body;
        }

        private void ParseDividend(BcsReportBody body, string value, Currencies? currency)
        {
            if (!currency.HasValue)
                throw new ArgumentNullException(nameof(currency));

            var info = _excel.GetCellValue(_rowId, 14);

            if (info is null)
                throw new Exception(nameof(ParseDividend) + ". Info not found");

            var dateTime = DateOnly.Parse(_excel.GetCellValue(_rowId, 1)!, _culture);
            var exchangeId = GetExchangeId();

            // дивиденд

            var derivativeId = new DerivativeId(currency.Value.ToString());
            var derivativeCode = new DerivativeCode(_derivatives[derivativeId.AsString][0]);

            body.AddEvent(new EventModel
            {
                DerivativeId = derivativeId,
                DerivativeCode = derivativeCode,

                EventTypeId = new EventTypeId(EventTypes.Dividend),
                Value = decimal.Parse(_excel.GetCellValue(_rowId, 6)!),

                Date = dateTime,
                Info = info,

                ExchangeId = exchangeId,
                AccountId = body.AccountId,
                UserId = _userId,
                ProviderId = _providerId
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

            body.AddEvent(new EventModel
            {
                DerivativeId = derivativeId,
                DerivativeCode = derivativeCode,

                EventTypeId = new EventTypeId(EventTypes.TaxIncome),
                Value = tax,

                Date = dateTime,
                Info = info,

                ExchangeId = exchangeId,
                AccountId = body.AccountId,
                UserId = _userId,
                ProviderId = _providerId
            });
        }
        private void ParseComission(BcsReportBody body, string value, Currencies? currency)
        {
            if (!currency.HasValue)
                throw new ArgumentNullException(nameof(currency));

            var derivativeId = new DerivativeId(currency.Value.ToString());
            var derivativeCode = new DerivativeCode(_derivatives[derivativeId.AsString][0]);

            body.AddEvent(new EventModel
            {
                DerivativeId = derivativeId,
                DerivativeCode = derivativeCode,

                EventTypeId = new EventTypeId(_reportPatterns[value].EventType),
                Value = decimal.Parse(_excel.GetCellValue(_rowId, 7)!),

                Date = DateOnly.Parse(_excel.GetCellValue(_rowId, 1)!, _culture),
                Info = value,

                AccountId = body.AccountId,
                UserId = _userId,
                ProviderId = _providerId,
                ExchangeId = GetExchangeId()
            });
        }
        private void ParseBalance(BcsReportBody body, string value, Currencies? currency)
        {
            if (!currency.HasValue)
                throw new ArgumentNullException(nameof(currency));

            var eventTypeId = new EventTypeId(_reportPatterns[value].EventType);

            var columnNo = eventTypeId.AsEnum switch
            {
                EventTypes.Increase => 6,
                EventTypes.Decrease => 7,
                EventTypes.InterestIncome => 6,
                EventTypes.Dividend => 6,
                _ => throw new ApplicationException(nameof(ParseBalance) + $".{nameof(EventType)} '{value}' not recognized")
            };

            var derivativeId = new DerivativeId(currency.Value.ToString());
            var derivativeCode = new DerivativeCode(_derivatives[derivativeId.AsString][0]);

            body.AddEvent(new EventModel
            {
                DerivativeId = derivativeId,
                DerivativeCode = derivativeCode,

                EventTypeId = eventTypeId,
                Value = decimal.Parse(_excel.GetCellValue(_rowId, columnNo)!),

                Date = DateOnly.Parse(_excel.GetCellValue(_rowId, 1)!, _culture),
                Info = value,

                AccountId = body.AccountId,
                UserId = _userId,
                ProviderId = _providerId,
                ExchangeId = GetExchangeId()
            });
        }
        private void ParseExchangeRate(BcsReportBody body, string value, Currencies? currency = null)
        {
            var code = _excel.GetCellValue(_rowId, 1);

            if (code is null)
                throw new ApplicationException(nameof(ParseExchangeRate) + ".Code not found");

            if (!BcsReportStructure.ExchangeCurrencies.ContainsKey(code))
                throw new ApplicationException(nameof(ParseExchangeRate) + $".Derivative '{code}' not found");

            var incomeDerivativeId = new DerivativeId(BcsReportStructure.ExchangeCurrencies[code].Income);
            var incomeDerivativeCode = new DerivativeCode(_derivatives[incomeDerivativeId.AsString][0]);

            var expenseDerivativeId = new DerivativeId(BcsReportStructure.ExchangeCurrencies[code].Expense);
            var expenseDerivativeCode = new DerivativeCode(_derivatives[expenseDerivativeId.AsString][0]);

            while (!_excel.TryGetCellValue(_rowId++, 1, $"Итого по {code}:", out _))
            {
                var cellBuyValue = _excel.GetCellValue(_rowId, 5);

                var date = DateOnly.Parse(_excel.GetCellValue(_rowId, 1)!, _culture);
                var exchange = _excel.GetCellValue(_rowId, 14);
                var exchangeId = !string.IsNullOrWhiteSpace(exchange) && BcsReportStructure.ExchangeTypes.ContainsKey(exchange)
                    ? new ExchangeId(BcsReportStructure.ExchangeTypes[exchange])
                    : throw new ApplicationException($"Не удалось определить площадку по значению: {exchange}");

                var dealId = new EntityStateId(Guid.NewGuid());
                IncomeModel incomeModel;
                ExpenseModel expenseModel;

                decimal dealValue;
                decimal dealCost;

                if (!string.IsNullOrWhiteSpace(cellBuyValue))
                {
                    dealCost = decimal.Parse(_excel.GetCellValue(_rowId, 4)!);
                    dealValue = decimal.Parse(cellBuyValue);
                    incomeModel = new IncomeModel(dealId, incomeDerivativeId, incomeDerivativeCode, dealValue, date);
                    expenseModel = new ExpenseModel(dealId, expenseDerivativeId, expenseDerivativeCode, dealValue * dealCost, date);
                }
                else
                {
                    dealCost = decimal.Parse(_excel.GetCellValue(_rowId, 7)!);
                    dealValue = decimal.Parse(_excel.GetCellValue(_rowId, 8)!);
                    incomeModel = new IncomeModel(dealId, incomeDerivativeId, incomeDerivativeCode, dealValue * dealCost, date);
                    expenseModel = new ExpenseModel(dealId, expenseDerivativeId, expenseDerivativeCode, dealValue, date);
                }

                var dealModel = new DealModel(dealId, incomeModel, expenseModel)
                {
                    Date = date,
                    Cost = dealCost,

                    UserId = _userId,
                    ProviderId = _providerId,
                    AccountId = body.AccountId,
                    ExchangeId = exchangeId,

                    Info = code
                };

                body.AddDeal(dealModel);
            }
        }
        private void ParseStockTransactions(BcsReportBody body, string value, Currencies? currency = null)
        {
            var isin = _excel.GetCellValue(_rowId, 7);

            if (isin is null)
                throw new ApplicationException(nameof(ParseStockTransactions) + ".Isin not found");

            var infoArray = isin.Split(',').Select(x => x.Trim());

            var derivativeId = new DerivativeId(_derivatives.Keys.Intersect(infoArray).FirstOrDefault());
            var derivativeCode = new DerivativeCode(_derivatives[derivativeId.AsString][0]);

            var name = _excel.GetCellValue(_rowId, 1);

            while (!_excel.TryGetCellValue(_rowId++, 1, $"Итого по {name}:", out _))
            {
                var cellBuyValue = _excel.GetCellValue(_rowId, 4);

                var date = DateOnly.Parse(_excel.GetCellValue(_rowId, 1)!, _culture);
                currency = _excel.GetCellValue(_rowId, 10) switch
                {
                    "USD" => Currencies.Usd,
                    "Рубль" => Currencies.Rub,
                    _ => throw new ArgumentOutOfRangeException(nameof(ParseStockTransactions) + $".Currency {currency} not found")
                };

                var exchange = _excel.GetCellValue(_rowId, 17);
                var exchangeId = !string.IsNullOrWhiteSpace(exchange) && BcsReportStructure.ExchangeTypes.ContainsKey(exchange)
                    ? new ExchangeId(BcsReportStructure.ExchangeTypes[exchange])
                    : throw new ApplicationException($"Не удалось определить площадку по значению: {exchange}");

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
        private void ParseAdditionalStockRelease(BcsReportBody body, string value, Currencies? currency = null)
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

                Date = DateOnly.Parse(_excel.GetCellValue(_rowId, 4)!, _culture),
                Info = _excel.GetCellValue(_rowId, 12),

                UserId = _userId,
                AccountId = body.AccountId,
                ProviderId = _providerId,
                ExchangeId = new ExchangeId(Exchanges.Spbex)
            });
        }

        private void CheckComission(string value)
        {
            if (!_reportPatterns.ContainsKey(value))
                throw new ApplicationException(nameof(CheckComission) + $".{nameof(EventType)} '{value}' not found");
        }
        private ExchangeId GetExchangeId()
        {
            var exchange = _excel.GetCellValue(_rowId, 12);
            if (string.IsNullOrWhiteSpace(exchange))
                exchange = _excel.GetCellValue(_rowId, 11);
            if (string.IsNullOrWhiteSpace(exchange))
                exchange = _excel.GetCellValue(_rowId, 10);

            return new ExchangeId(string.IsNullOrWhiteSpace(exchange) ? Exchanges.Default : BcsReportStructure.ExchangeTypes[exchange]);
        }
    }
}