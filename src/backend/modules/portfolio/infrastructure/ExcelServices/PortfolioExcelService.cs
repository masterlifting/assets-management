using AM.Services.Portfolio.Core.Abstractions.ExcelService;

using Shared.Documents.Excel;

namespace AM.Services.Portfolio.Infrastructure.ExcelServices;

public class PortfolioExcelService : IPortfolioExcelService
{
    IPortfolioExcelDocument IPortfolioExcelService.GetExcelDocument(byte[] payload)
    {
        var table = ExcelService.LoadTable(payload);
        return new PortfolioExcelDocument(table);
    }
}