namespace AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Companies;

public sealed record BcsCompaniesResult
{
    public ICollection<BcsAsset> Companies { get; init; } = Array.Empty<BcsAsset>();
}
