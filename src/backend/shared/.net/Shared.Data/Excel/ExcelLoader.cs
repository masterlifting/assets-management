﻿using System.Data;
using System.Text;

using ExcelDataReader;

namespace Shared.Data.Excel;

public class ExcelLoader
{
    public static DataTable LoadTable(byte[] data)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using var stream = new MemoryStream(data);
        using var reader = ExcelReaderFactory.CreateBinaryReader(stream);
        var dataSet = reader.AsDataSet();
        var table = dataSet.Tables[0];
        stream.Close();
        return table;
    }
}