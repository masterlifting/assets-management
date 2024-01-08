namespace AM.Services.Portfolio.Core;

public static class Constants
{
    public static class Enums
    {
        public enum EventTypes
        {
            Adding = 1,
            Withdrawing,

            BankInvestments,
            CrowdlendingInvestments,
            CrowdfundingInvestments,
            VentureInvestments,

            InterestProfit,
            InvestmentProfit,
            InvestmentBody,

            Splitting,
            Donation,

            Dividend,
            Coupon,

            Delisting,
            Loss,
            TaxCountry,
            TaxDeal,
            TaxIncome,
            TaxProvider,
            TaxDepositary,
            ComissionDeal,
            ComissionProvider,
            ComissionDepositary
        }
        public enum OperationTypes
        {
            Income = 1,
            Expense
        }
        public enum Providers
        {
            Safe = 1,
            Bcs,
            Tinkoff,
            Vtb,
            JetLend,
            Bitokk,
            XChange
        }
        public enum ProcessSteps
        {
            ParseBcsReport = 1,
            CalculateEvent,
            SendEvent,
            CalculateDeal,
            SendDeal
        }
    }
}