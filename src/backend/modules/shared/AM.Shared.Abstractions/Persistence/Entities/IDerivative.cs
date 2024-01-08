namespace AM.Shared.Abstractions.Persistence.Entities;

public interface IDerivative
{
    string Ticker { get; init; }
    int AssetId { get; init; }
    int ZoneId { get; init; }
}