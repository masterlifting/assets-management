using static AM.Services.Common.Constants.Enums;
using static AM.Services.Portfolio.Core.Constants.Enums;

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
    public static readonly Dictionary<string, (EventTypes eventType, int columnNo)> BalanceEvents = new(StringComparer.OrdinalIgnoreCase)
    {
        {"Приход ДС",( EventTypes.Adding , 6) },
        {"Вывод ДС",( EventTypes.Withdrawing , 7) },
        {"Возмещение дивидендов по сделке",( EventTypes.InvestmentProfit , 6) },
        {"Проценты по займам \"овернайт\"",( EventTypes.InterestProfit , 6) },
        {"Проценты по займам \"овернайт ЦБ\"",( EventTypes.InterestProfit , 6) }
    };
    public static readonly Dictionary<string, EventTypes> ComissionEvents = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Урегулирование сделок", EventTypes.ComissionProvider },
        { "Вознаграждение компании", EventTypes.ComissionProvider },
        { "Вознаграждение за обслуживание счета депо", EventTypes.ComissionDepositary },
        { "Хранение ЦБ", EventTypes.ComissionDepositary },
        { "НДФЛ", EventTypes.TaxCountry },
        { "Вознаграждение компании (СВОП)", EventTypes.ComissionProvider },
        { "Комиссия за займы \"овернайт ЦБ\"", EventTypes.ComissionProvider },
        { "Вознаграждение компании (репо)", EventTypes.ComissionProvider },
        { "Комиссия Биржевой гуру", EventTypes.ComissionProvider },
        { "Оплата за вывод денежных средств", EventTypes.ComissionProvider },
        { "Распределение (4*)", EventTypes.ComissionProvider }
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