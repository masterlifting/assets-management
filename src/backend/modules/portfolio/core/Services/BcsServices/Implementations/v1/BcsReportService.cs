using AM.Services.Portfolio.Core.Abstractions.ExcelService;
using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.BcsServices.Implementations.Helpers;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;
using AM.Services.Portfolio.Core.Services.BcsServices.Models;

using static AM.Services.Common.Constants.Enums;
using static AM.Services.Portfolio.Core.Constants.Enums;
using static Shared.Persistence.Abstractions.Constants.Enums;

namespace AM.Services.Portfolio.Core.Services.BcsServices.Implementations.v1;

public sealed class BcsReportService : IBcsReportService
{
    private const int ProviderId = (int)Providers.Bcs;

    private const string Initiator = nameof(BcsReportService);

    private readonly IPortfolioExcelService _excelService;
    private readonly IUnitOfWorkRepository _uow;

    private readonly Dictionary<string, Action<int, IPortfolioExcelDocument, string, Currencies?, List<BcsReportEventModel>>> _eventPatterns;
    private readonly Dictionary<string, Func<int, IPortfolioExcelDocument, string, List<BcsReportDealModel>, int>> _dealPatterns;

    public BcsReportService(IPortfolioExcelService excelService, IUnitOfWorkRepository uow)
    {
        _excelService = excelService;
        _uow = uow;
        _eventPatterns = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Приход ДС", ParseBalance },
            { "Вывод ДС", ParseBalance },
            { "Возмещение дивидендов по сделке", ParseBalance },
            { "Проценты по займам \"овернайт\"", ParseBalance },
            { "Проценты по займам \"овернайт ЦБ\"", ParseBalance },

            { "Дивиденды", ParseDividend },

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
        _dealPatterns = new(StringComparer.OrdinalIgnoreCase)
        {
            { "ISIN:", ParseTransactions },
            { "Сопряж. валюта:", ParseExchangeRate }
        };
    }

    public BcsReportModel GetReportModel(string fileName, byte[] payload)
    {
        BcsReportHelper.CheckFile(fileName, nameof(GetReportModel));

        var excel = _excelService.GetExcelDocument(payload);

        int rowId = 0;

        var agreement = BcsReportHelper.GetReportAgreement(rowId, excel);
        var (dateStart, dateEnd) = BcsReportHelper.GetReportPeriod(rowId, excel);

        var fileStructure = BcsReportHelper.GetFileStructure(rowId, excel);

        List<BcsReportEventModel> events = new(excel.RowsCount / 2);
        List<BcsReportDealModel> deals = new(excel.RowsCount / 2);

        string? patternKey;

        var firstBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points.FirstBlock, StringComparison.OrdinalIgnoreCase) > -1);
        if (firstBlock is not null)
        {
            rowId = fileStructure[firstBlock];

            var border = fileStructure.Skip(1).First().Key;

            while (!excel.TryGetCellValue(++rowId, 1, border, out patternKey))
                if (patternKey is not null)
                    switch (patternKey)
                    {
                        case "USD":
                            InvokeAction(patternKey, Currencies.Usd);
                            break;
                        case "Рубль":
                            InvokeAction(patternKey, Currencies.Rub);
                            break;
                    }

            void InvokeAction(string value, Currencies currency)
            {
                while (!excel.TryGetCellValue(++rowId, 1, new[] { $"Итого по валюте {value}:", border }, out _))
                {
                    patternKey = excel.GetCellValue(rowId, 2);

                    if (patternKey is not null && _eventPatterns.ContainsKey(patternKey))
                        _eventPatterns[patternKey](rowId, excel, patternKey, currency, events);
                }
            }
        }

        var comissionsBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points.ComissionsBlock, StringComparison.OrdinalIgnoreCase) > -1);
        if (comissionsBlock is not null)
        {
            rowId = fileStructure[comissionsBlock] + 3;

            while (!excel.TryGetCellValue(++rowId, 1, "Итого по валюте Рубль:", out patternKey))
                if (patternKey is not null && !BcsReportFileStructure.ComissionEvents.ContainsKey(patternKey))
                    throw new PortfolioCoreException(Initiator, "Cheacking comission", new($"Comission: '{patternKey}' was not recognezed"));
        }

        var dealsBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points.DealsBlock, StringComparison.OrdinalIgnoreCase) > -1);
        if (dealsBlock is not null)
        {
            rowId = fileStructure[dealsBlock];

            var borders = fileStructure.Keys
                .Where(x =>
                    BcsReportFileStructure.Points.UnfinishedDealsBlock.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1
                    || BcsReportFileStructure.Points.AssetsBlock.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1
                    || BcsReportFileStructure.Points.LastBlock.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1)
                .ToArray();

            while (!excel.TryGetCellValue(++rowId, 1, borders, out _))
            {
                patternKey = excel.GetCellValue(rowId, 6);

                if (patternKey is not null && _dealPatterns.ContainsKey(patternKey))
                    _dealPatterns[patternKey](rowId, excel, patternKey, deals);
            }
        }

        var lastBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsReportFileStructure.Points.LastBlock, StringComparison.OrdinalIgnoreCase) > -1);
        if (lastBlock is not null)
        {
            rowId = fileStructure[lastBlock];

            while (!excel.TryGetCellValue(++rowId, 1, "Дата составления отчета:", out _))
            {
                patternKey = excel.GetCellValue(rowId, 12);

                if (patternKey is not null
                    && patternKey.Length > 5
                    && !int.TryParse(patternKey[0..3], out _)
                    && !BcsReportFileStructure.LastBlockExceptedWords.Any(x => patternKey.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                {
                    if (_eventPatterns.Keys.Any(x => patternKey.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                    {
                        var keyWord = string.Join(' ', patternKey.Split(' ')[0..2]);
                        var value = excel.GetCellValue(rowId, 1)!;

                        _eventPatterns[keyWord](rowId, excel, value, null, events);
                    }
                    else if (_dealPatterns.Keys.Any(x => patternKey.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                    {
                        var keyWord = string.Join(' ', patternKey.Split(' ')[0..2]);
                        var value = excel.GetCellValue(rowId, 1)!;

                        _dealPatterns[keyWord](rowId, excel, value, deals);
                    }
                    else
                        throw new PortfolioCoreException(Initiator, $"Parsing '{lastBlock}'", new($"PatternKey '{patternKey}' was not recognezed in the row '{rowId + 1}'"));
                }
            }
        }

        return new BcsReportModel
        {
            Agreement = agreement,
            DateStart = dateStart,
            DateEnd = dateEnd,
            Events = events,
            Deals = deals
        };
    }

    public async Task<Event[]> GetEventsAsync(Guid userId, string agreement, IList<BcsReportEventModel> models, CancellationToken cToken = default)
    {
        var result = new List<Event>(models.Count);

        Account account = await _uow.Account.GetAccountAsync(agreement, userId, ProviderId);
        Derivative[] derivatives = await _uow.Derivative.GetDerivativesAsync(AssetTypes.Stock);
        Dictionary<string, IEnumerable<Derivative>> derivativeDictionary = derivatives
            .GroupBy(x => x.Code)
            .ToDictionary(x => x.Key, x => x.AsEnumerable());

        foreach (var item in models)
        {
            if(!derivativeDictionary.ContainsKey(item.Asset))
                throw new PortfolioCoreException(Initiator, nameof(GetEventsAsync), new($"'Asset '{item.Asset}' was not recognized as derivative"));

            var derivative = derivativeDictionary[item.Asset].First();

                result.Add(new()
                {
                    UserId = userId,
                    AccountId = account.Id,
                    TypeId = (int)item.EventType,
                    ProviderId = ProviderId,
                    DerivativeId = derivative.Id,
                    ExchangeId = (int)item.Exchange,
                    ProcessStatusId = (int)ProcessStatuses.Ready,
                    ProcessStepId = (int)ProcessSteps.CalculateEvent,

                    Date = DateOnly.FromDateTime(item.Date),
                    Value = item.Value
                });
        }

        return result.ToArray();
    }
    public async Task<Deal[]> GetDealsAsync(Guid userId, string agreement, IList<BcsReportDealModel> models, CancellationToken cToken = default)
    {
        var result = new List<Deal>(models.Count);

        Account account = await _uow.Account.GetAccountAsync(agreement, userId, ProviderId);
        Derivative[] derivatives = await _uow.Derivative.GetDerivativesAsync();
        Dictionary<string, IEnumerable<Derivative>> derivativeDictionary = derivatives
            .GroupBy(x => x.Code)
            .ToDictionary(x => x.Key, x => x.AsEnumerable());

        foreach (var item in models)
        {
            if (!derivativeDictionary.ContainsKey(item.IncomeEvent.Asset))
                throw new PortfolioCoreException(Initiator, nameof(GetDealsAsync), new($"'Income asset '{item.IncomeEvent.Asset}' was not recognized as derivative"));
            if (!derivativeDictionary.ContainsKey(item.ExpenseEvent.Asset))
                throw new PortfolioCoreException(Initiator, nameof(GetDealsAsync), new($"'Expense asset '{item.ExpenseEvent.Asset}' was not recognized as derivative"));

            var derivativeIncome = derivativeDictionary[item.IncomeEvent.Asset].First();
            var derivativeExpense = derivativeDictionary[item.ExpenseEvent.Asset].First();
            
            var dealId = Guid.NewGuid();
            
            result.Add(new()
            {
                Id = dealId,
                UserId = userId,
                AccountId = account.Id,
                Cost = item.IncomeEvent.Value * item.ExpenseEvent.Value,
                Income = new()
                { 
                    DealId = dealId,
                    DerivativeId = derivativeIncome.Id,
                    Date = DateOnly.FromDateTime(item.IncomeEvent.Date),
                    Value = item.IncomeEvent.Value
                },
                Expense = new()
                {
                    DealId = dealId,
                    DerivativeId = derivativeExpense.Id,
                    Date = DateOnly.FromDateTime(item.ExpenseEvent.Date),
                    Value = item.ExpenseEvent.Value
                },
                ProcessStatusId = (int)ProcessStatuses.Ready,
                ProcessStepId = (int)ProcessSteps.CalculateDeal
            });
        }

        return result.ToArray();
    }

    #region Events Parsers
    private static void ParseBalance(int rowId, IPortfolioExcelDocument excel, string value, Currencies? currency, List<BcsReportEventModel> events)
    {
        var action = nameof(ParseBalance);

        if (!BcsReportFileStructure.BalanceEvents.ContainsKey(value))
            throw new PortfolioCoreException(Initiator, action, new($"Event type '{value}' not recognized"));

        var eventData = BcsReportHelper.GetBalanceEventData(value, action);

        events.Add(new()
        {
            Asset = BcsReportHelper.GetCurrency(currency, action),
            Value = BcsReportHelper.GetDecimal(excel.GetCellValue(rowId, eventData.ColumnNo), action),
            Date = BcsReportHelper.GetDate(excel.GetCellValue(rowId, 1), action),
            EventType = eventData.EventType,
            Exchange = BcsReportHelper.GetExchange(rowId, excel, action)
        });
    }
    private static void ParseDividend(int rowId, IPortfolioExcelDocument excel, string value, Currencies? currency, List<BcsReportEventModel> events)
    {
        var action = nameof(ParseDividend);

        var info = excel.GetCellValue(rowId, 14);
        if (info is null)
            throw new PortfolioCoreException(Initiator, action, new("Description not found"));

        var _currency = BcsReportHelper.GetCurrency(currency, action);
        var exchange = BcsReportHelper.GetExchange(rowId, excel, action);
        var date = BcsReportHelper.GetDate(excel.GetCellValue(rowId, 1), action);

        events.Add(new()
        {
            Asset = _currency,
            Value = BcsReportHelper.GetDecimal(excel.GetCellValue(rowId, 6), action),
            Date = date,
            EventType = EventTypes.Dividend,
            Exchange = exchange,
            Info = info
        });

        var taxPosition = info.IndexOf("налог", StringComparison.OrdinalIgnoreCase);

        if (taxPosition <= -1)
            return;

        var taxSumData = info[taxPosition..].Split(' ')[1];
        var taxSum = BcsReportHelper.GetDecimal(taxSumData.IndexOf('$') > -1 ? taxSumData[1..] : taxSumData, action);

        events.Add(new()
        {
            Asset = _currency,
            Value = taxSum,
            Date = date,
            EventType = EventTypes.TaxIncome,
            Exchange = exchange,
            Info = info
        });
    }
    private static void ParseComission(int rowId, IPortfolioExcelDocument excel, string value, Currencies? currency, List<BcsReportEventModel> events)
    {
        var action = nameof(ParseComission);

        events.Add(new()
        {
            Asset = BcsReportHelper.GetCurrency(currency, action),
            Value = BcsReportHelper.GetDecimal(excel.GetCellValue(rowId, 7), action),
            Date = BcsReportHelper.GetDate(excel.GetCellValue(rowId, 1), action),
            EventType = BcsReportHelper.GetComissionEventData(value, action),
            Exchange = BcsReportHelper.GetExchange(rowId, excel, action)
        });
    }
    private static void ParseStockShare(int rowId, IPortfolioExcelDocument excel, string value, Currencies? currency, List<BcsReportEventModel> events)
    {
        var action = nameof(ParseStockShare);

        events.Add(new()
        {
            Asset = value.Trim(),
            Value = BcsReportHelper.GetDecimal(excel.GetCellValue(rowId, 7), action),
            Date = BcsReportHelper.GetDate(excel.GetCellValue(rowId, 4), action),
            Exchange = BcsReportHelper.GetExchange(rowId, excel, action),
            EventType = EventTypes.Donation
        });
    }
    private static void ParseStockSplit(int rowId, IPortfolioExcelDocument excel, string value, Currencies? currency, List<BcsReportEventModel> events)
    {
        var action = nameof(ParseStockSplit);

        var valueBefore = BcsReportHelper.GetInteger(excel.GetCellValue(rowId, 6), action);
        var valueAfter = BcsReportHelper.GetInteger(excel.GetCellValue(rowId, 7), action);

        var splitValue = valueAfter / valueBefore;

        events.Add(new()
        {
            Asset = value.Trim(),
            Value = splitValue,
            Date = BcsReportHelper.GetDate(excel.GetCellValue(rowId, 4), action),
            Exchange = BcsReportHelper.GetExchange(rowId, excel, action),
            EventType = EventTypes.Splitting
        });
    }
    #endregion
    #region Deals Parsers
    private static int ParseExchangeRate(int rowId, IPortfolioExcelDocument excel, string value, List<BcsReportDealModel> deals)
    {
        var action = nameof(ParseExchangeRate);

        var currency = excel.GetCellValue(rowId, 1);
        var (incomeCurrency, expenseCurrency) = BcsReportHelper.GetExchangeCurrencies(currency, action);

        while (!excel.TryGetCellValue(++rowId, 1, $"Итого по {currency}:", out _))
        {
            var incomeValueString = excel.GetCellValue(rowId, 5);
            if (incomeValueString is not null)
            {
                var incomeValue = BcsReportHelper.GetDecimal(incomeValueString, action);
                var exchange = BcsReportHelper.GetExchange(rowId, excel, action);
                var date = BcsReportHelper.GetDate(excel.GetCellValue(rowId, 1), action);
                var decreasingValue = BcsReportHelper.GetDecimal(excel.GetCellValue(rowId, 4), action);

                deals.Add(new()
                {
                    IncomeEvent = new()
                    {
                        Asset = incomeCurrency,
                        Value = incomeValue,
                        Date = date,
                        Exchange = exchange
                    },
                    ExpenseEvent = new()
                    {
                        Asset = expenseCurrency,
                        Value = decreasingValue * incomeValue,
                        Date = date,
                        Exchange = exchange
                    }
                });

                continue;
            }

            var expenseValueString = excel.GetCellValue(rowId, 8);
            if (expenseValueString is not null)
            {
                var expenseValue = BcsReportHelper.GetDecimal(expenseValueString, action);
                var exchange = BcsReportHelper.GetExchange(rowId, excel, action);
                var date = BcsReportHelper.GetDate(excel.GetCellValue(rowId, 1), action);
                var increasingValue = BcsReportHelper.GetDecimal(excel.GetCellValue(rowId, 7), action);

                deals.Add(new()
                {
                    ExpenseEvent = new()
                    {
                        Asset = expenseCurrency,
                        Value = expenseValue,
                        Date = date,
                        Exchange = exchange
                    },
                    IncomeEvent = new()
                    {
                        Asset = incomeCurrency,
                        Value = increasingValue * expenseValue,
                        Date = date,
                        Exchange = exchange
                    }
                });
            }
        }

        return rowId;
    }
    private static int ParseTransactions(int rowId, IPortfolioExcelDocument excel, string value, List<BcsReportDealModel> deals)
    {
        var action = nameof(ParseTransactions);

        var isin = excel.GetCellValue(rowId, 7);
        if (isin is null)
            throw new PortfolioCoreException(Initiator, nameof(ParseTransactions), new("ISIN not recognized"));

        var asset = excel.GetCellValue(rowId, 1);

        while (!excel.TryGetCellValue(++rowId, 1, $"Итого по {asset}:", out _))
        {
            var incomeValueString = excel.GetCellValue(rowId, 4);
            if (incomeValueString is not null)
            {
                var incomeValue = BcsReportHelper.GetDecimal(incomeValueString, action);
                var exchange = BcsReportHelper.GetExchange(rowId, excel, action);
                var date = BcsReportHelper.GetDate(excel.GetCellValue(rowId, 1), action);

                var currencyCode = excel.GetCellValue(rowId, 10);
                if (currencyCode is null)
                    throw new PortfolioCoreException(Initiator, nameof(ParseTransactions), new("Currency not found"));
                var currencyName = currencyCode switch
                {
                    "USD" => Currencies.Usd.ToString(),
                    "Рубль" => Currencies.Rub.ToString(),
                    _ => throw new PortfolioCoreException(Initiator, nameof(ParseTransactions), new("Currency not recognized"))
                };

                var decreasingValue = BcsReportHelper.GetDecimal(excel.GetCellValue(rowId, 5), action);

                deals.Add(new()
                {
                    IncomeEvent = new()
                    {
                        Asset = isin,
                        Value = incomeValue,
                        Date = date,
                        Exchange = exchange
                    },
                    ExpenseEvent = new()
                    {
                        Asset = currencyName,
                        Value = decreasingValue * incomeValue,
                        Date = date,
                        Exchange = exchange
                    }
                });

                continue;
            }

            var expenseValueString = excel.GetCellValue(rowId, 7);
            if (expenseValueString is not null)
            {
                var expenseValue = BcsReportHelper.GetDecimal(expenseValueString, action);
                var exchange = BcsReportHelper.GetExchange(rowId, excel, action);
                var date = BcsReportHelper.GetDate(excel.GetCellValue(rowId, 1), action);

                var currencyCode = excel.GetCellValue(rowId, 10);
                if (currencyCode is null)
                    throw new PortfolioCoreException(Initiator, nameof(ParseTransactions), new("Currency not found"));
                var currencyName = currencyCode switch
                {
                    "USD" => Currencies.Usd.ToString(),
                    "Рубль" => Currencies.Rub.ToString(),
                    _ => throw new PortfolioCoreException(Initiator, nameof(ParseTransactions), new("Cirrency not recognized"))
                };

                var increasingValue = BcsReportHelper.GetDecimal(excel.GetCellValue(rowId, 8), action);

                deals.Add(new()
                {
                    ExpenseEvent = new()
                    {
                        Asset = isin,
                        Value = expenseValue,
                        Date = date,
                        Exchange = exchange
                    },
                    IncomeEvent = new()
                    {
                        Asset = currencyName,
                        Value = increasingValue * expenseValue,
                        Date = date,
                        Exchange = exchange
                    }
                });
            }
        }

        return rowId;
    }
    #endregion
}
