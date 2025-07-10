using AM.Portfolio.Core.Abstractions.Documents.Excel;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Companies;
using AM.Portfolio.Core.Exceptions;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Companies;
using AM.Portfolio.Core.Services.BcsServices.Implementations.Helpers;
using AM.Portfolio.Core.Services.DataHeapServices.Bcs.Helpers;

namespace AM.Portfolio.Core.Services.DataHeapServices.Bcs.Companies;

public sealed class BcsCompaniesParser : IBcsCompaniesParser
{
    private readonly IPortfolioExcelDocumentService _excelService;
    public BcsCompaniesParser(IPortfolioExcelDocumentService excelService) => _excelService = excelService;

    public BcsCompaniesResult Parse(string source, byte[] payload)
    {
        BcsHelper.ApproveReport(source);

        var document = _excelService.Load(payload);

        var fileStructure = BcsHelper.GetFileStructure(document, 0);

        List<BcsAsset> companies = new(100);

        var rowId = 0;

        ParseData(ref rowId, document, fileStructure, companies);
        CheckData(ref rowId, document, fileStructure, companies);

        return new()
        {
            Companies = companies
        };
    }

    private static void ParseData(ref int rowId, IPortfolioExcelDocument document, Dictionary<string, int> fileStructure, List<BcsAsset> assets)
    {
        var companiesBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsFileStructure.Points.Companies, StringComparison.OrdinalIgnoreCase) > -1);

        if (companiesBlock is not null)
        {
            rowId = fileStructure[companiesBlock];

            var borders = new[] { BcsFileStructure.Points.AssetsMoving, BcsFileStructure.Points.End };

            while (!document.TryGetCell(++rowId, 1, borders, out var ticker))
            {
                if (!document.TryGetCell(rowId, 16, out var name) || name == "Эмитент")
                    continue;

                if (!document.TryGetCell(rowId, 3, out var code))
                    continue;

                var tickerIndex = ticker.IndexOf(string.Intern(" (в пути)"), StringComparison.OrdinalIgnoreCase);

                if (tickerIndex > -1)
                    ticker = ticker.AsSpan(0, tickerIndex).ToString();

                assets.Add(new(name, ticker, code));
            }
        }
    }
    private static void CheckData(ref int rowId, IPortfolioExcelDocument document, Dictionary<string, int> fileStructure, List<BcsAsset> assets)
    {
        var lastBlock = fileStructure.Keys.FirstOrDefault(x => x.IndexOf(BcsFileStructure.Points.AssetsMoving, StringComparison.OrdinalIgnoreCase) > -1);

        if (lastBlock is not null)
        {
            rowId = fileStructure[lastBlock];

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
                            throw new PortfolioCoreException($"Parsing block '{lastBlock}' by event '{eventCellValue}' was not recognized in the row '{rowId + 1}'.");

                        var assetTicker = assetCellValue.TrimEnd();

                        var asset =
                            assets.FirstOrDefault(x => x.Ticker.IndexOf(assetTicker, StringComparison.OrdinalIgnoreCase) > -1)
                            ?? throw new PortfolioCoreException($"Parsing block '{lastBlock}'. Asset '{assetTicker}' from event '{eventCellValue}' was not recognized in the row '{rowId + 1}'.");
                    }
                    else
                    {
                        throw new PortfolioCoreException($"Parsing block '{lastBlock}' by event '{eventCellValue}' was not recognized in the row '{rowId + 1}'.");
                    }
                }
            }
        }
    }
}
