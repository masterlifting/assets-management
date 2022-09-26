using static AM.Services.Common.Contracts.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Deserialization.Reports.Bcs.Models;

internal static class BcsReportFileStructure
{
    internal static readonly string[] Points =
    {
        "1.1.1. Движение денежных средств по совершенным сделкам (иным операциям) с ценными бумагами",
        "1.2. Займы:",
        "сборы/штрафы (итоговые суммы):",
        "2.1. Сделки:",
        "2.3. Незавершенные сделки",
        "3. Активы:"
    };
    internal static readonly Dictionary<string, Exchanges> ExchangeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "МосБирж(Валютный рынок)", Exchanges.Moex },
        { "ММВБ", Exchanges.Moex },
        { "СПБ", Exchanges.Spbex },
        { "МосБирж(FORTS)", Exchanges.Moex },
        { "Внебирж.", Exchanges.Moex }
    };
    internal static readonly Dictionary<string, (string Income, string Expense)> ExchangeCurrencies = new(StringComparer.OrdinalIgnoreCase)
    {
        {  "USDRUB_TOD", ("USD", "RUB") },
        {  "USDRUB_TOM", ("USD","RUB") },
        {  "EURRUB_TOM", ("EUR", "RUB") },
        {  "EURRUB_TOD", ("EUR", "RUB") },
        {  "EURUSD_TOD", ("EUR", "USD")},
        {  "EURUSD_TOM", ("EUR", "USD") }
    };
}