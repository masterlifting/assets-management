namespace AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Transactions;

public sealed record BcsDeal
{
    public BcsEvent Income { get; init; } = null!;
    public BcsEvent Expense { get; init; } = null!;
}
