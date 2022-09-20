namespace AM.Services.Portfolio.Core;

public static class Constants
{
    public static class Persistense
    {
        public static class Enums
        {
            public enum EventTypes
            {
                Default = -1,

                Increase = 1,
                Decrease = 2,

                BankInvestments = 3,
                CrowdlendingInvestments = 4,
                CrowdfundingInvestments = 5,
                VentureInvestments = 6,

                InterestIncome = 7,
                InvestmentIncome = 8,
                InvestmentBody = 9,

                Split = 10,
                Dividend = 11,
                Coupon = 12,

                Delisting = 13,
                Loss = 14,
                Ndfl = 15,
                TaxDeal = 16,
                TaxIncome = 17,
                TaxProvider = 18,
                TaxDepositary = 19
            }
            public enum OperationTypes
            {
                Default = -1,
                Income = 1,
                Expense = 2
            }
            public enum Providers
            {
                Safe = 1,
                Bcs = 2,
                Tinkoff = 3,
                Vtb = 4,
                JetLend = 5,
                Bitokk = 6,
                XChange = 7
            }
        }
    }
}