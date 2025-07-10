using System.Globalization;
using System.Text.RegularExpressions;

using AM.Portfolio.Core.Abstractions.Documents.Excel;
using AM.Portfolio.Core.Exceptions;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs;
using AM.Portfolio.Core.Services.DataHeapServices.Bcs.Helpers;

using Net.Shared.Extensions;

using static AM.Shared.Models.Constants.Enums;

namespace AM.Portfolio.Core.Services.BcsServices.Implementations.Helpers;

internal static partial class BcsHelper
{
    private static readonly CultureInfo Culture = new("Ru-ru");

    internal static void ApproveReport(string fileName)
    {
        var match = FileNameRegex().Match(fileName);

        if (!match.Success)
            throw new PortfolioCoreException($"The '{fileName}' was not recognized as a BCS report.");
    }

    internal static Dictionary<string, int> GetFileStructure(IPortfolioExcelDocument excel, int rowId)
    {
        var fileStructurePoints = new string[]
        {
            BcsFileStructure.Points.MoneyMoving
            , BcsFileStructure.Points.Loans
            , BcsFileStructure.Points.Commissions
            , BcsFileStructure.Points.Deals
            , BcsFileStructure.Points.UnfinishedDeals
            , BcsFileStructure.Points.Assets
            , BcsFileStructure.Points.Companies
            , BcsFileStructure.Points.AssetsMoving
        };

        var result = new Dictionary<string, int>(fileStructurePoints.Length);

        while (!excel.TryGetCell(++rowId, 1, "Дата составления отчета:", out var cellValue))
        {
            if (fileStructurePoints.Any(x => cellValue.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
            {
                result[cellValue] = rowId;
            }
        }

        return !result.Any()
            ? throw new PortfolioCoreException("The structure of the report was not recognized.")
            : result;
    }

    internal static string GetAgreement(IPortfolioExcelDocument excel, int rowId)
    {
        while (!excel.TryGetCell(++rowId, 1, "Генеральное соглашение:", out _))
            continue;

        return !excel.TryGetCell(rowId, 5, out var agreement)
            ? throw new PortfolioCoreException($"The agreement was not found.")
            : agreement;
    }

    internal static (DateTime DateStart, DateTime DateEnd) GetPeriod(IPortfolioExcelDocument excel, int rowId)
    {
        while (!excel.TryGetCell(++rowId, 1, "Период:", out _))
            continue;

        if (!excel.TryGetCell(rowId, 5, out var period))
            throw new PortfolioCoreException("Period of the report was not found.");

        var periods = period.Split('\u0020');

        return (GetDateTime(periods[1]), GetDateTime(periods[3]));
    }

    internal static (BcsAsset Income, BcsAsset Expense) GetExchangeRates(string? value) => value is null
        ? throw new PortfolioCoreException("The exchange rate pair was not found.")
        : BcsFileStructure.ReportExchangeRates.TryGetValue(value, out var key)
            ? key
            : throw new PortfolioCoreException($"The exchange rate pair '{value}' was not recognized.");

    internal static Exchanges GetExchange(IPortfolioExcelDocument excel, int rowId)
    {
        for (var columnNo = 10; columnNo < 20; columnNo++)
        {
            if (excel.TryGetCell(rowId, columnNo, out var exchange)
                && exchange != "0"
                && !exchange[0].ToString().TryParse<int>(out _)
                && BcsFileStructure.ReportExchanges.TryGetValue(exchange, out var result))
            {
                return result;
            }
        }

        throw new PortfolioCoreException($"The exchange from the row number {rowId + 1} was not recognized.");
    }

    internal static decimal GetDecimal(string? value) => value is null
        ? throw new PortfolioCoreException("The decimal value was not found.")
            : !value.TryParse<decimal>(out var result)
            ? throw new PortfolioCoreException($"The decimal value '{value}' was not recognized.")
        : result;

    internal static int GetInteger(string? value) => value is null
        ? throw new PortfolioCoreException("The integer value was not found.")
            : !value.TryParse<int>(out var result)
            ? throw new PortfolioCoreException($"The integer value '{value}' was not recognized.")
        : result;

    internal static DateTime GetDateTime(string? value) => value is null
        ? throw new PortfolioCoreException("The datetime value was not found.")
            : !DateTime.TryParse(value, Culture, DateTimeStyles.None, out var result)
            ? throw new PortfolioCoreException($"The datetime value '{value}' was not recognized.")
        : result;

    [GeneratedRegex("^B_k-(.+)_ALL(.+).xls$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex FileNameRegex();
}
