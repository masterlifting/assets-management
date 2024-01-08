namespace AM.Portfolio.Core;

public static class Constants
{
    public static class Enums
    {
        public enum BcsReportEvents
        {
            Income,
            Expense,
            IncomeDividend,
            IncomePercentage,
            CommissionBroker,
            CommissionDepositary,
            TaxZone,
            Splitting,
            Sharing,
        }
        public enum EventTypes
        {
            TopUp,
            Withdraw,
            Percentage,
            Donation,
            Tax,
            Commission,
            Bankruptcy,
            Fail,
            Splitting,
            Multiplying,
        }
        public enum Holders
        {
            Cash = 1,
            LedgerNanoX,
            Bcs,
            Vtb,
            JetLend,
            Qiwi,
            RaiffeisenRussia,
            RaiffeisenSerbia,
            PostanskaStedionica,
            ExpobankSerbia
        }
        public enum ProcessSteps
        {
            None = 1,
            CalculateSplitting,
            CalculateBalance,
            ParseBcsCompanies,
            ParseBcsTransactions,
            ParseRaiffeisenSrbTransactions,
        }
    }
}
