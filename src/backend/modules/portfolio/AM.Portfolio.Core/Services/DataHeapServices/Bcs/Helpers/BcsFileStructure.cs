using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs;
using static AM.Portfolio.Core.Constants.Enums;
using static AM.Shared.Models.Constants.Assets;
using static AM.Shared.Models.Constants.Enums;

namespace AM.Portfolio.Core.Services.DataHeapServices.Bcs.Helpers;

public static class BcsFileStructure
{
    public static class Points
    {
        public const string MoneyMoving = "1.1.1. Движение денежных средств по совершенным сделкам (иным операциям) с ценными бумагами";
        public const string Loans = "1.2. Займы:";
        public const string Commissions = "сборы/штрафы (итоговые суммы)";
        public const string Deals = "2.1. Сделки:";
        public const string UnfinishedDeals = "2.3. Незавершенные сделки";
        public const string Assets = "3. Активы:";
        public const string Companies = "Портфель по ценным бумагам";
        public const string AssetsMoving = "4. Движение Ценных бумаг";
        public const string End = "Дата составления отчета:";
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

    public static readonly Dictionary<string, BcsReportEvents> ReportEvents = new(StringComparer.OrdinalIgnoreCase)
    {
        {"Приход ДС", BcsReportEvents.Income },
        {"Вывод ДС", BcsReportEvents.Expense },

        { "Дивиденды", BcsReportEvents.IncomeDividend },
        {"Возмещение дивидендов по сделке", BcsReportEvents.IncomeDividend },

        { "Доп. выпуск", BcsReportEvents.Sharing },
        { "Сплит акций", BcsReportEvents.Splitting },

        {"Проценты по займам \"овернайт\"", BcsReportEvents.IncomePercentage },
        {"Проценты по займам \"овернайт ЦБ\"", BcsReportEvents.IncomePercentage },

        { "Урегулирование сделок", BcsReportEvents.CommissionBroker },
        { "Вознаграждение компании", BcsReportEvents.CommissionBroker },
        { "Вознаграждение за обслуживание счета депо", BcsReportEvents.CommissionDepositary },
        { "Хранение ЦБ", BcsReportEvents.CommissionBroker },
        { "НДФЛ", BcsReportEvents.TaxZone },
        { "Вознаграждение компании (СВОП)", BcsReportEvents.CommissionBroker },
        { "Комиссия за займы \"овернайт ЦБ\"", BcsReportEvents.CommissionBroker },
        { "Вознаграждение компании (репо)", BcsReportEvents.CommissionBroker },
        { "Комиссия Биржевой гуру", BcsReportEvents.CommissionBroker },
        { "Оплата за вывод денежных средств", BcsReportEvents.CommissionBroker },
        { "Распределение (4*)", BcsReportEvents.CommissionBroker }
    };
    public static readonly Dictionary<string, Exchanges> ReportExchanges = new(StringComparer.OrdinalIgnoreCase)
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
    public static readonly Dictionary<string, (BcsAsset Income, BcsAsset Expense)> ReportExchangeRates = new(StringComparer.OrdinalIgnoreCase)
    {
        {  "USDRUB_TOD", (new(Usd,"USD"), new(Rub, "RUB")) },
        {  "USDRUB_TOM", (new(Usd, "USD"), new(Rub, "RUB")) },
        {  "EURRUB_TOM", (new(Eur, "EUR"), new(Rub, "RUB")) },
        {  "EURRUB_TOD", (new(Eur, "EUR"), new(Rub, "RUB")) },
        {  "EURUSD_TOD", (new(Eur, "EUR"), new(Usd, "USD"))},
        {  "EURUSD_TOM", (new(Eur, "EUR"), new(Usd, "USD")) }
    };
}
