namespace AM.Services.Common.Contracts.Models.Entity.Interfaces;

public interface IQuarterIdentity : IPeriod
{
    int Year { get; set; }
    byte Quarter { get; set; }
}