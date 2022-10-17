﻿using System.Data;

namespace Shared.Data.Excel;

public sealed class ExcelDocument
{
    public int RowsCount { get; }
    private readonly DataTable _table;
    public ExcelDocument(DataTable table)
    {
        _table = table;
        RowsCount = table.Rows.Count;
    }

    public string? GetCellValue(int rowId, int columnId)
    {
        var value = _table.Rows[rowId].ItemArray[columnId]?.ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
    public bool TryGetCellValue(int rowId, int columnId, string pattern, out string? value)
    {
        value = null;

        var cellValue = GetCellValue(rowId, columnId);
        var result = cellValue is not null && cellValue.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) > -1;

        if (result)
            value = cellValue!;

        return result;
    }
    public bool TryGetCellValue(int rowId, int columnId, IEnumerable<string> patterns, out string? value)
    {
        value = null;

        var cellValue = GetCellValue(rowId, columnId);
        var result = cellValue is not null && patterns.Any(x => cellValue.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1);

        if (result)
            value = cellValue!;

        return result;
    }
}