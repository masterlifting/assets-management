using AM.Portfolio.Core.Abstractions.Documents.Excel;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Exceptions;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Services.BcsServices.Implementations.Helpers;
using AM.Portfolio.Core.Services.DataHeapServices.Bcs.Helpers;
using AM.Portfolio.Core.Services.DataHeapServices.Bcs.Transactions.Parsers;

using static AM.Shared.Models.Constants;

namespace AM.Portfolio.Core.Services.DataHeapServices.Bcs.Transactions;

public sealed class BcsTransactionsParser : IBcsTransactionsParser
{
    private readonly IPortfolioExcelDocumentService _excelService;
    public BcsTransactionsParser(IPortfolioExcelDocumentService excelService) => _excelService = excelService;

    public BcsTransactionsResult Parse(string source, byte[] payload)
    {
        BcsHelper.ApproveReport(source);

        var document = _excelService.Load(payload);

        var agreement = BcsHelper.GetAgreement(document, 0);
        var (dateStart, dateEnd) = BcsHelper.GetPeriod(document, 0);

        var fileStructure = BcsHelper.GetFileStructure(document, 0);

        var eventParser = new BcsEventParser(document);
        var dealParser = new BcsDealParser(document);

        var rowId = 0;

        // The order is important
        ParseMoneyMoving(ref rowId, document, fileStructure, eventParser);
        ParseCommissions(ref rowId, document, fileStructure);
        ParseDeals(ref rowId, document, fileStructure, dealParser);
        ParseAssetsMoving(ref rowId, document, fileStructure, eventParser);

        return new()
        {
            Source = source,
            Agreement = agreement,
            DateStart = dateStart,
            DateEnd = dateEnd,
            Events = eventParser.Events,
            Deals = dealParser.Deals
        };
    }

    private static void ParseMoneyMoving(ref int rowId, IPortfolioExcelDocument document, Dictionary<string, int> fileStructure, BcsEventParser parser)
    {
        var block = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsFileStructure.Points.MoneyMoving, StringComparison.OrdinalIgnoreCase) > -1);

        if (block is not null)
        {
            rowId = fileStructure[block];

            var border = fileStructure.Skip(1).First().Key;

            while (!document.TryGetCell(++rowId, 1, border, out var cellValue))
            {
                if (!string.IsNullOrEmpty(cellValue))
                {
                    BcsAsset? asset = cellValue switch
                    {
                        "USD" => new(Assets.Usd, "USD"),
                        "Рубль" => new(Assets.Rub, "RUB"),
                        _ => null
                    };

                    if (asset is null)
                        continue;

                    var eventBorders = new[] { $"Итого по валюте {cellValue}:", border };

                    while (!document.TryGetCell(++rowId, 1, eventBorders, out _))
                    {
                        if (document.TryGetCell(rowId, 2, out cellValue))
                            parser.Parse(cellValue, rowId, asset);
                    }
                }
            }
        }
    }
    private static void ParseCommissions(ref int rowId, IPortfolioExcelDocument document, Dictionary<string, int> fileStructure)
    {
        var block = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsFileStructure.Points.Commissions, StringComparison.OrdinalIgnoreCase) > -1);

        if (block is not null)
        {
            rowId = fileStructure[block] + 3;

            var border = string.Intern("Итого по валюте Рубль:");

            while (!document.TryGetCell(++rowId, 1, border, out var cellValue))
            {
                if (!string.IsNullOrEmpty(cellValue) && !BcsFileStructure.ReportEvents.ContainsKey(cellValue))
                    throw new PortfolioCoreException($"The commission '{cellValue}' was not recognized.");
            }
        }
    }
    private static void ParseDeals(ref int rowId, IPortfolioExcelDocument document, Dictionary<string, int> fileStructure, BcsDealParser parser)
    {
        var block = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsFileStructure.Points.Deals, StringComparison.OrdinalIgnoreCase) > -1);

        if (block is not null)
        {
            rowId = fileStructure[block];

            var borders = fileStructure.Keys
                .Where(x =>
                    BcsFileStructure.Points.UnfinishedDeals.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1
                    || BcsFileStructure.Points.Assets.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1
                    || BcsFileStructure.Points.AssetsMoving.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1)
                .ToArray();

            while (!document.TryGetCell(++rowId, 1, borders, out _))
            {
                if (document.TryGetCell(rowId, 6, out var cellValue))
                    rowId = parser.Parse(cellValue, rowId);
            }
        }
    }
    private static void ParseAssetsMoving(ref int rowId, IPortfolioExcelDocument document, Dictionary<string, int> fileStructure, BcsEventParser parser)
    {
        var block = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsFileStructure.Points.AssetsMoving, StringComparison.OrdinalIgnoreCase) > -1);

        if (block is not null)
        {
            rowId = fileStructure[block];

            while (!document.TryGetCell(++rowId, 1, BcsFileStructure.Points.End, out _))
            {
                if (document.TryGetCell(rowId, 12, out var eventCellValue)
                    && eventCellValue.Length > 5
                    && !int.TryParse(eventCellValue[0..3], out _)
                    && !BcsFileStructure.LastBlockExceptedWords.Any(x => eventCellValue.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                {
                    if (BcsFileStructure.ReportEvents.Keys.Any(x => eventCellValue.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                    {
                        var eventPatternKey = string.Join(' ', eventCellValue.Split(' ')[0..2]);

                        if (!document.TryGetCell(rowId, 1, out var assetCellValue))
                            throw new PortfolioCoreException($"Parsing block '{block}' by event '{eventCellValue}' was not recognized in the row '{rowId + 1}'.");

                        var ticker = assetCellValue.TrimEnd();

                        parser.Parse(eventPatternKey, rowId, new BcsAsset(string.Empty, ticker));
                    }
                    else
                    {
                        throw new PortfolioCoreException($"Parsing block '{block}' by event '{eventCellValue}' was not recognized in the row '{rowId + 1}'.");
                    }
                }
            }
        }
    }
}
