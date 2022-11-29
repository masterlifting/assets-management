namespace AM.Services.Portfolio.Core.Abstractions.Excel;

public interface IPortfolioExcelService
{
    IPortfolioExcelDocument GetExcelDocument(byte[] payload);
}