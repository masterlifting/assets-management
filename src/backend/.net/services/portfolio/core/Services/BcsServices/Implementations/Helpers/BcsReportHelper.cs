using AM.Services.Portfolio.Core.Abstractions.ExcelService;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.BcsServices.Implementations.v1;

using System.Globalization;
using System.Text.RegularExpressions;

using static AM.Services.Common.Constants.Enums;
using static AM.Services.Portfolio.Core.Constants.Enums;

namespace AM.Services.Portfolio.Core.Services.BcsServices.Implementations.Helpers
{
    internal static class BcsReportHelper
    {
        private static readonly CultureInfo Culture = new("Ru-ru");
        private const string Initiator = nameof(BcsReportService);

        internal static void CheckFile(string fileName, string action)
        {
            action = action + '.' + nameof(CheckFile);

            var match = Regex.Match(fileName, "^B_k-(.+)_ALL(.+).xls$", RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new PortfolioCoreException(Initiator, action, new($"'File '{fileName}' was not recognized for the BCS"));
        }

        internal static Dictionary<string, int> GetFileStructure(int rowId, IPortfolioExcelDocument excel)
        {
            var fileStructurePoints = new string[]
            {
                BcsReportFileStructure.Points.FirstBlock
                , BcsReportFileStructure.Points.LoansBlock
                , BcsReportFileStructure.Points.ComissionsBlock
                , BcsReportFileStructure.Points.DealsBlock
                , BcsReportFileStructure.Points.UnfinishedDealsBlock
                , BcsReportFileStructure.Points.AssetsBlock
                , BcsReportFileStructure.Points.LastBlock
            };

            var result = new Dictionary<string, int>(fileStructurePoints.Length);

            while (!excel.TryGetCellValue(++rowId, 1, "Дата составления отчета:", out var cellValue))
                if (cellValue is not null && fileStructurePoints.Any(x => cellValue.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1))
                    result.Add(cellValue, rowId);

            return !result.Any()
                ? throw new PortfolioCoreException(Initiator, nameof(GetFileStructure), new("The structure of the file was not recognized"))
                : result;
        }
        internal static string GetReportAgreement(int rowId, IPortfolioExcelDocument excel)
        {
            while (!excel.TryGetCellValue(++rowId, 1, "Генеральное соглашение:", out _))
                continue;
            
            var agreement = excel.GetCellValue(rowId, 5);

            return string.IsNullOrWhiteSpace(agreement)
                ? throw new PortfolioCoreException(Initiator, nameof(GetReportAgreement), new("The agreement was not found"))
                : agreement;
        }
        internal static (DateTime DateStart, DateTime DateEnd) GetReportPeriod(int rowId, IPortfolioExcelDocument excel)
        {
            var action = nameof(GetReportPeriod);

            while (!excel.TryGetCellValue(++rowId, 1, "Период:", out _))
                continue;

            var period = excel.GetCellValue(rowId, 5);

            if (string.IsNullOrWhiteSpace(period))
                throw new PortfolioCoreException(Initiator, action, new("The period was not found"));

            var periods = period.Split('\u0020');

            return (GetDate(periods[1], action), GetDate(periods[3], action));
        }
        
        internal static (string Income, string Expense) GetExchangeCurrencies(string? value, string action)
        {
            action = action + '.' + nameof(GetExchangeCurrencies);

            return value is null
            ? throw new PortfolioCoreException(Initiator, action, new("Value not found"))
            : BcsReportFileStructure.ExchangeCurrencies.ContainsKey(value)
                ? BcsReportFileStructure.ExchangeCurrencies[value]
                : throw new PortfolioCoreException(Initiator, action, new($"Value '{value}' was not recognized"));
        }
        internal static Exchanges GetExchange(int rowId, IPortfolioExcelDocument excel, string action)
        {
            action = action + '.' + nameof(GetExchange);

            for (var columnNo = 10; columnNo < 20; columnNo++)
            {
                var exchange = excel.GetCellValue(rowId, columnNo);

                if (!string.IsNullOrEmpty(exchange)
                    && exchange != "0"
                    && !int.TryParse(exchange[0].ToString(), out _)
                    && BcsReportFileStructure.ExchangeTypes.ContainsKey(exchange))
                    return BcsReportFileStructure.ExchangeTypes[exchange];
            }

            throw new PortfolioCoreException(Initiator, action, new($"The exchange name was not recognized in the row number: {rowId + 1}"));
        }
        internal static (EventTypes EventType, int ColumnNo) GetBalanceEventData(string value, string action)
        {
            action = action + '.' + nameof(GetBalanceEventData);

            return BcsReportFileStructure.BalanceEvents.ContainsKey(value)
                ? BcsReportFileStructure.BalanceEvents[value]
                : throw new PortfolioCoreException(Initiator, action, new($"Event type '{value}' was not recognized"));
        }
        internal static EventTypes GetComissionEventData(string value, string action)
        {
            action = action + '.' + nameof(GetComissionEventData);

            return BcsReportFileStructure.ComissionEvents.ContainsKey(value)
                ? BcsReportFileStructure.ComissionEvents[value]
                : throw new PortfolioCoreException(Initiator, action, new($"Event type '{value}' was not recognized"));
        }
      
        internal static string GetCurrency(Currencies? value, string action) => !value.HasValue
            ? throw new PortfolioCoreException(Initiator, action +'.'+ nameof(GetCurrency), new("Currency not found"))
            : value.Value.ToString();
        internal static decimal GetDecimal(string? value, string action)
        {
            action = action + '.' + nameof(GetDecimal);

            return value is null
            ? throw new PortfolioCoreException(Initiator, action, new("Value not found"))
                : !decimal.TryParse(value, out var result)
                ? throw new PortfolioCoreException(Initiator, action, new($"Value '{value}' was not recognized"))
            : result;
        }
        internal static int GetInteger(string? value, string action)
        {
            action = action + '.' + nameof(GetInteger);

            return value is null
            ? throw new PortfolioCoreException(Initiator, action, new("Value not found"))
                : !int.TryParse(value, out var result)
                ? throw new PortfolioCoreException(Initiator, action, new($"Value '{value}' was not recognized"))
            : result;
        }
        internal static DateTime GetDate(string? value, string action)
        {
            action = action + '.' + nameof(GetDate);

            return value is null
            ? throw new PortfolioCoreException(Initiator, action, new("Value not found"))
                : !DateTime.TryParse(value, Culture, DateTimeStyles.None, out var result)
                ? throw new PortfolioCoreException(Initiator, action, new($"Value '{value}' was not recognized"))
            : result;
        }
    }
}
