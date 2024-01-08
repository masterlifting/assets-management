using AM.Portfolio.Core.Abstractions.Documents.Excel;

using Net.Shared.Documents.Abstractions.Excel;

namespace AM.Portfolio.Infrastructure.Documents.ExcelDocumentServices;

public sealed class PortfolioExcelDocument : IPortfolioExcelDocument
{
    private readonly IExcelDocument _document;

    public PortfolioExcelDocument(IExcelDocument document)
    {
        _document = document;
        RowsCount = _document.RowsCount;
    }
    public int RowsCount { get; }

    public bool TryGetCell(int rowId, int columnId, out string value) => 
        _document.TryGetCell(rowId, columnId, out value);

    public bool TryGetCell(int rowId, int columnId, string pattern, out string value) => 
        _document.TryGetCell(rowId, columnId, pattern, out value);

    public bool TryGetCell(int rowId, int columnId, IEnumerable<string> patterns, out string value) => 
        _document.TryGetCell(rowId, columnId, patterns, out value);
}
