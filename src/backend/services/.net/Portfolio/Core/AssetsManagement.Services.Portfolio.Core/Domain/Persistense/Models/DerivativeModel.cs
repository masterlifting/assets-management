using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models;

public record DerivativeModel
{
    public DerivativeModel(DerivativeId derivativeId, DerivativeCode derivativeCode, AssetId assetId, AssetTypeId assetTypeId, decimal balance)
    {
        DerivativeId = derivativeId;
        DerivativeCode = derivativeCode;
        AssetId = assetId;
        AssetTypeId = assetTypeId;
        Balance = balance;
    }
    public DerivativeId DerivativeId { get; } = null!;
    public DerivativeCode DerivativeCode { get; } = null!;

    public AssetId AssetId { get; } = null!;
    public AssetTypeId AssetTypeId { get; } = null!;

    public decimal Balance { get; }

    public Derivative GetEntity() => new()
    {
        Id = DerivativeId.AsString,
        Code = DerivativeCode.AsString,

        AssetId = AssetId.AsString,
        AssetTypeId = AssetTypeId.AsInt,

        Balance = Balance,
        UpdateTime = DateTime.UtcNow
    };
}