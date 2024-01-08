using AM.Portfolio.Core.Abstractions.Documents.Excel;

using Net.Shared.Documents.Abstractions.Excel;

namespace AM.Portfolio.Infrastructure.Documents.ExcelDocumentServices;

internal class PortfolioExcelDocumentService : IPortfolioExcelDocumentService
{
    private readonly IExcelDocumentService _service;

    public PortfolioExcelDocumentService(IExcelDocumentService service) => _service = service;
    public IPortfolioExcelDocument Load(byte[] document)
    {
        var excelDocument = _service.Load(document);
        return new PortfolioExcelDocument(excelDocument);
    }
}
