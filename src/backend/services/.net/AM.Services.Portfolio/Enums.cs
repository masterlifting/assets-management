namespace AM.Services.Portfolio;

public static class Enums
{
    public enum Providers
    {
        Safe = 1,
        BCS = 2,
        Tinkoff = 3,
        VTB = 4,
        JetLend = 5,
        Bitokk= 6,
        XChange= 7
    }
    public enum OperationTypes : byte
    {
        Default = 255,
        Income = 1,
        Expense = 2
    }
    public enum EventTypes : byte
    {
        Default = 255,
        
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
        NDFL = 15,
        TaxDeal = 16,
        TaxIncome = 17,
        TaxProvider = 18,
        TaxDepositary = 19
    }
    public enum Steps : byte
    {
        Parsing
    }
}