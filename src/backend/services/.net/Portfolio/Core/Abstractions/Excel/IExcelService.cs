namespace AM.Services.Portfolio.Core.Abstractions.Excel;

public interface IExcelService
{
    IExcelDocument GetExcelDocument(byte[] payload);
}
