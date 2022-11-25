namespace AM.Services.Portfolio.Core.Abstractions.Excel;

public interface IExcelDocument
{
    int RowsCount { get; }

    string? GetCellValue(int rowId, int columnId);
    bool TryGetCellValue(int rowId, int columnId, IEnumerable<string> patterns, out string? value);
    bool TryGetCellValue(int rowId, int columnId, string pattern, out string? value);
}
