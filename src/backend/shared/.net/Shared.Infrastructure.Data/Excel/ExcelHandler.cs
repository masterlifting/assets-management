using System.Data;

namespace Shared.Infrastructure.Data.Excel;

public class ExcelHandler
{
    public int RowsCount { get;}
    private readonly DataTable _table;
    public ExcelHandler(DataTable table)
    {
        _table = table;
        RowsCount = table.Rows.Count;
    }

    public string? GetCellValue(int rowId, int columnId)
    {
        var value = _table.Rows[rowId].ItemArray[columnId]?.ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
    public bool TryGetCellValue(int rowId, int columnId, string pattern, out string cell)
    {
        cell = string.Empty;
        
        var cellValue = GetCellValue(rowId, columnId);
        var result = cellValue is not null && cellValue.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) > -1;

        if (result)
            cell = cellValue!;

        return result;
    }
    public bool TryGetCellValue(int rowId, int columnId, IEnumerable<string> patterns, out string cell)
    {
        cell = string.Empty;

        var cellValue = GetCellValue(rowId, columnId);
        var result = cellValue is not null && patterns.Any(x => cellValue.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1);
        
        if(result)
            cell = cellValue!;
        
        return result;
    }
}