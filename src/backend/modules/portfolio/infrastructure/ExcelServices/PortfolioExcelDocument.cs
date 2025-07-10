using AM.Services.Portfolio.Core.Abstractions.ExcelService;

using Shared.Documents.Excel;

using System.Data;

namespace AM.Services.Portfolio.Infrastructure.ExcelServices;

internal class PortfolioExcelDocument : IPortfolioExcelDocument
{
    public int RowsCount { get; }
    private readonly ExcelDocument _document;
    public PortfolioExcelDocument(DataTable table)
    {
        _document = new ExcelDocument(table);
        RowsCount = _document.RowsCount;
    }

    public string? GetCellValue(int rowId, int columnId) => _document.GetCellValue(rowId, columnId);
    public bool TryGetCellValue(int rowId, int columnId, IEnumerable<string> patterns, out string? value) => _document.TryGetCellValue(rowId, columnId, patterns, out value);
    public bool TryGetCellValue(int rowId, int columnId, string pattern, out string? value) => _document.TryGetCellValue(rowId, columnId, pattern, out value);
}