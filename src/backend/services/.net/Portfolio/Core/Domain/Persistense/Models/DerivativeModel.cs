using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models;

public sealed record DerivativeModel
{
    public DerivativeModel(DerivativeId derivativeId, DerivativeCode derivativeCode, AssetId assetId, AssetTypeId assetTypeId, decimal balance)
    {
        DerivativeId = derivativeId;
        DerivativeCode = derivativeCode;
        AssetId = assetId;
        AssetTypeId = assetTypeId;
        BalanceValue = balance;
    }
    public DerivativeId DerivativeId { get; } = null!;
    public DerivativeCode DerivativeCode { get; } = null!;

    public AssetId AssetId { get; } = null!;
    public AssetTypeId AssetTypeId { get; } = null!;

    public decimal BalanceValue { get; }

    public Derivative GetEntity() => new()
    {
        Id = DerivativeId.AsString,
        Code = DerivativeCode.AsString,

        AssetId = AssetId.AsString,
        AssetTypeId = AssetTypeId.AsInt,

        BalanceValue = BalanceValue,
        UpdateTime = DateTime.UtcNow
    };
}