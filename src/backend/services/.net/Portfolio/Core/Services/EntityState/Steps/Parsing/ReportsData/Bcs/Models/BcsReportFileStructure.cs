using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs.Models;

internal static class BcsReportFileStructure
{
    internal static readonly string[] Points =
    {
        "1.1.1. Движение денежных средств по совершенным сделкам (иным операциям) с ценными бумагами",
        "1.2. Займы:",
        "сборы/штрафы (итоговые суммы)",
        "2.1. Сделки:",
        "2.3. Незавершенные сделки",
        "3. Активы:",
        "4. Движение Ценных бумаг"
    };
    internal static readonly Dictionary<string, (string eventType, int columnNo)> BalanceEvents = new(StringComparer.OrdinalIgnoreCase)
    {
        {"Приход ДС",( nameof(EventTypes.Increase) , 6) },
        {"Вывод ДС",( nameof(EventTypes.Decrease) , 7) },
        {"Возмещение дивидендов по сделке",( nameof(EventTypes.InvestmentProfit) , 6) },
        {"Проценты по займам \"овернайт\"",( nameof(EventTypes.InterestProfit) , 6) },
        {"Проценты по займам \"овернайт ЦБ\"",( nameof(EventTypes.InterestProfit) , 6) }
    };
    internal static readonly Dictionary<string, string> ComissionEvents = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Урегулирование сделок", nameof(EventTypes.TaxProvider) },
        { "Вознаграждение компании", nameof(EventTypes.TaxProvider) },
        { "Вознаграждение за обслуживание счета депо", nameof(EventTypes.TaxDepositary) },
        { "Хранение ЦБ", nameof(EventTypes.TaxDepositary) },
        { "НДФЛ", nameof(EventTypes.TaxCountry) },
        { "Вознаграждение компании (СВОП)", nameof(EventTypes.TaxProvider) },
        { "Комиссия за займы \"овернайт ЦБ\"", nameof(EventTypes.TaxProvider) },
        { "Вознаграждение компании (репо)", nameof(EventTypes.TaxProvider) },
        { "Комиссия Биржевой гуру", nameof(EventTypes.TaxProvider) },
        { "Оплата за вывод денежных средств", nameof(EventTypes.TaxProvider) },
        { "Распределение (4*)", nameof(EventTypes.TaxProvider) }
    };
    internal static readonly string[] FourthBlockExceptedWords =
    {
        "погашен"
        , "состояние"
        , "сумма нкд"
        , "прочее"
        , "примечание"
        , "Зачисление"
        , "Списание"
        , "Конвертация"
    };
    internal static readonly Dictionary<string, Exchanges> ExchangeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "МосБирж(Валютный рынок)", Exchanges.Moex },
        { "МосБирж(ВР/ДМ)", Exchanges.Moex },
        { "ММВБ", Exchanges.Moex },
        { "СПБ", Exchanges.Spbex },
        { "МосБирж(FORTS)", Exchanges.Moex },
        { "Внебирж.", Exchanges.Moex },
        { "Торговый раздел (НРД)", Exchanges.Moex },
        { "Торговый раздел (БЭБ)", Exchanges.Spbex },
        { "Торговый раздел (РДЦ)", Exchanges.Spbex },
        { "Торговый раздел (СПБ Банк)", Exchanges.Spbex },
        { "Блокированный раздел (СПБ Банк)", Exchanges.Spbex }
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