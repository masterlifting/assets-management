namespace AM.Services.Portfolio.Core.Abstractions.ExcelService;

public interface IPortfolioExcelService
{
    IPortfolioExcelDocument GetExcelDocument(byte[] payload);
}