using AM.Portfolio.Core.Abstractions.Documents.Excel;
using AM.Portfolio.Core.Exceptions;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Services.BcsServices.Implementations.Helpers;

using static AM.Shared.Models.Constants;

namespace AM.Portfolio.Core.Services.DataHeapServices.Bcs.Transactions.Parsers;

internal sealed class BcsDealParser
{
    public ICollection<BcsDeal> Deals { get; }

    private readonly List<BcsDeal> _deals;
    private readonly IPortfolioExcelDocument _document;
    private readonly Dictionary<string, Func<int, int>> _patterns;
    internal BcsDealParser(IPortfolioExcelDocument document)
    {
        _deals = new List<BcsDeal>(100);
        Deals = _deals;

        _document = document;
        _patterns = new(StringComparer.OrdinalIgnoreCase)
        {
            { "ISIN:", ParseTransactions },
            { "Сопряж. валюта:", ParseExchangeRate }
        };
    }

    public int Parse(string patternKey, int rowId) => _patterns.TryGetValue(patternKey, out var handler)
        ? handler.Invoke(rowId)
        : rowId;

    private int ParseExchangeRate(int rowId)
    {
        if (!_document.TryGetCell(rowId, 1, out var currency))
            throw new PortfolioCoreException($"The event ExchangeRate was not recognized for the exchange rate parsing.");

        var (incomeCurrency, expenseCurrency) = BcsHelper.GetExchangeRates(currency);

        while (!_document.TryGetCell(++rowId, 1, $"Итого по {currency}:", out _))
        {
            var isIncome = _document.TryGetCell(rowId, 5, out var cellValue);

            if (!isIncome)
            {
                _document.TryGetCell(rowId, 8, out cellValue);
            }

            if (!string.IsNullOrWhiteSpace(cellValue))
            {
                var dealValue = BcsHelper.GetDecimal(cellValue);
                var exchange = BcsHelper.GetExchange(_document, rowId);

                if (!_document.TryGetCell(rowId, 1, out var dateCellValue))
                    throw new PortfolioCoreException($"The event ExchangeRate was not recognized for the exchange rate parsing.");

                var date = BcsHelper.GetDateTime(dateCellValue);

                if (isIncome)
                {
                    if (!_document.TryGetCell(rowId, 4, out var targetCellValue))
                        throw new PortfolioCoreException($"The event ExchangeRate was not recognized for the exchange rate parsing.");

                    var targetDealValue = BcsHelper.GetDecimal(targetCellValue) * dealValue;

                    _deals.Add(new()
                    {
                        Income = new()
                        {
                            Asset = incomeCurrency,
                            Value = dealValue,
                            Date = date,
                            Exchange = exchange
                        },
                        Expense = new()
                        {
                            Asset = expenseCurrency,
                            Value = targetDealValue,
                            Date = date,
                            Exchange = exchange
                        }
                    });
                }
                else
                {
                    if (!_document.TryGetCell(rowId, 7, out var targetCellValue))
                        throw new PortfolioCoreException($"The event ExchangeRate was not recognized for the exchange rate parsing.");

                    var targetDealValue = BcsHelper.GetDecimal(targetCellValue) * dealValue;

                    _deals.Add(new()
                    {
                        Expense = new()
                        {
                            Asset = expenseCurrency,
                            Value = dealValue,
                            Date = date,
                            Exchange = exchange
                        },
                        Income = new()
                        {
                            Asset = incomeCurrency,
                            Value = targetDealValue,
                            Date = date,
                            Exchange = exchange
                        }
                    });
                }
            }
        }

        return rowId;
    }
    private int ParseTransactions(int rowId)
    {
        if (!_document.TryGetCell(rowId, 7, out var companyCode))
            throw new PortfolioCoreException("The ISIN was not found.");

        if (!_document.TryGetCell(rowId, 1, out var companyTicker))
            throw new PortfolioCoreException($"The ticker for the ISIN '{companyCode}' was not found.");

        if (!_document.TryGetCell(rowId, 8, out var companyName))
            throw new PortfolioCoreException($"The company name for the ticker '{companyTicker}' was not found.");

        var asset = new BcsAsset(companyName, companyTicker, companyCode);

        while (!_document.TryGetCell(++rowId, 1, $"Итого по {companyTicker}:", out _))
        {
            var isIncome = _document.TryGetCell(rowId, 4, out var cellValue);

            if (!isIncome)
            {
                _document.TryGetCell(rowId, 7, out cellValue);
            }

            if (!string.IsNullOrEmpty(cellValue))
            {
                var dealValue = BcsHelper.GetDecimal(cellValue);
                var exchange = BcsHelper.GetExchange(_document, rowId);

                if (!_document.TryGetCell(rowId, 1, out var dateCellValue))
                    throw new PortfolioCoreException("The date was not found.");

                var date = BcsHelper.GetDateTime(dateCellValue);

                if (!_document.TryGetCell(rowId, 10, out var currencyCode))
                    throw new PortfolioCoreException("The currency code was not found.");

                var currencyAsset = currencyCode switch
                {
                    "USD" => new BcsAsset(Assets.Usd, "USD"),
                    "Рубль" => new BcsAsset(Assets.Rub, "RUB"),
                    _ => throw new PortfolioCoreException($"The currency code '{currencyCode}' was not recognized.")
                };

                if (isIncome)
                {
                    if (!_document.TryGetCell(rowId, 5, out var targetCellValue))
                        throw new PortfolioCoreException("The decreasing value was not found.");

                    var targetDealValue = BcsHelper.GetDecimal(targetCellValue) * dealValue;

                    _deals.Add(new()
                    {
                        Income = new()
                        {
                            Asset = asset,
                            Value = dealValue,
                            Date = date,
                            Exchange = exchange
                        },
                        Expense = new()
                        {
                            Asset = currencyAsset,
                            Value = targetDealValue,
                            Date = date,
                            Exchange = exchange
                        }
                    });
                }
                else
                {
                    if (!_document.TryGetCell(rowId, 8, out var targetCellValue))
                        throw new PortfolioCoreException("The increasing value was not found.");

                    var targetDealValue = BcsHelper.GetDecimal(targetCellValue) * dealValue;

                    _deals.Add(new()
                    {
                        Expense = new()
                        {
                            Asset = asset,
                            Value = dealValue,
                            Date = date,
                            Exchange = exchange
                        },
                        Income = new()
                        {
                            Asset = currencyAsset,
                            Value = targetDealValue,
                            Date = date,
                            Exchange = exchange
                        }
                    });
                }
            }
        }

        return rowId;
    }
}
