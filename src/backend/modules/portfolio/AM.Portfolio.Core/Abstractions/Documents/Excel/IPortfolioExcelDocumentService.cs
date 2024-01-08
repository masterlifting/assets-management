namespace AM.Portfolio.Core.Abstractions.Documents.Excel;

public interface IPortfolioExcelDocumentService
{
    IPortfolioExcelDocument Load(byte[] document);
}
