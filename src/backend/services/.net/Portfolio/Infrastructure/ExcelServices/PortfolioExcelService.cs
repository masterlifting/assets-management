using AM.Services.Portfolio.Core.Abstractions.Excel;

using Shared.Data.Excel;

namespace AM.Services.Portfolio.Infrastructure.Excel;

public class PortfolioExcelService : IPortfolioExcelService
{
    IPortfolioExcelDocument IPortfolioExcelService.GetExcelDocument(byte[] payload)
    {
        var table = ExcelService.LoadTable(payload);
        return new PortfolioExcelDocument(table);
    }
}
