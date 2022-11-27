using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.BcsServices.Implementations.Helpers;

public static class BcsReportFileStructure
{
    public static class Points
    {
        public const string FirstBlock = "1.1.1. Движение денежных средств по совершенным сделкам (иным операциям) с ценными бумагами";
        public const string LoansBlock = "1.2. Займы:";
        public const string ComissionsBlock = "сборы/штрафы (итоговые суммы)";
        public const string DealsBlock = "2.1. Сделки:";
        public const string UnfinishedDealsBlock = "2.3. Незавершенные сделки";
        public const string AssetsBlock = "3. Активы:";
        public const string LastBlock = "4. Движение Ценных бумаг";
    };
    public static readonly Dictionary<string, (string eventType, int columnNo)> BalanceEvents = new(StringComparer.OrdinalIgnoreCase)
    {
        {"Приход ДС",( nameof(EventTypes.Adding) , 6) },
        {"Вывод ДС",( nameof(EventTypes.Withdrawing) , 7) },
        {"Возмещение дивидендов по сделке",( nameof(EventTypes.InvestmentProfit) , 6) },
        {"Проценты по займам \"овернайт\"",( nameof(EventTypes.InterestProfit) , 6) },
        {"Проценты по займам \"овернайт ЦБ\"",( nameof(EventTypes.InterestProfit) , 6) }
    };
    public static readonly Dictionary<string, string> ComissionEvents = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Урегулирование сделок", nameof(EventTypes.ComissionProvider) },
        { "Вознаграждение компании", nameof(EventTypes.ComissionProvider) },
        { "Вознаграждение за обслуживание счета депо", nameof(EventTypes.ComissionDepositary) },
        { "Хранение ЦБ", nameof(EventTypes.ComissionDepositary) },
        { "НДФЛ", nameof(EventTypes.TaxCountry) },
        { "Вознаграждение компании (СВОП)", nameof(EventTypes.ComissionProvider) },
        { "Комиссия за займы \"овернайт ЦБ\"", nameof(EventTypes.ComissionProvider) },
        { "Вознаграждение компании (репо)", nameof(EventTypes.ComissionProvider) },
        { "Комиссия Биржевой гуру", nameof(EventTypes.ComissionProvider) },
        { "Оплата за вывод денежных средств", nameof(EventTypes.ComissionProvider) },
        { "Распределение (4*)", nameof(EventTypes.ComissionProvider) }
    };
    public static readonly string[] LastBlockExceptedWords =
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
    public static readonly Dictionary<string, Exchanges> ExchangeTypes = new(StringComparer.OrdinalIgnoreCase)
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
    public static readonly Dictionary<string, (string Income, string Expense)> ExchangeCurrencies = new(StringComparer.OrdinalIgnoreCase)
    {
        {  "USDRUB_TOD", ("USD", "RUB") },
        {  "USDRUB_TOM", ("USD","RUB") },
        {  "EURRUB_TOM", ("EUR", "RUB") },
        {  "EURRUB_TOD", ("EUR", "RUB") },
        {  "EURUSD_TOD", ("EUR", "USD")},
        {  "EURUSD_TOM", ("EUR", "USD") }
    };
}