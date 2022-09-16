namespace AM.Services.Portfolio.Core.Interfaces.Persistense.Entities;

public interface IBalance
{
    decimal Balance { get; set; }
    decimal BalanceCost { get; set; }
    decimal LastDealCost { get; set; }
}