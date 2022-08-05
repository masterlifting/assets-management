using System.Collections.Generic;

namespace AM.Services.Common.Contracts.Models.Entity.Interfaces;

public interface IAssetType<TAsset> : ICatalog where TAsset : class, IAsset
{
    IEnumerable<TAsset>? Assets { get; set; }
}