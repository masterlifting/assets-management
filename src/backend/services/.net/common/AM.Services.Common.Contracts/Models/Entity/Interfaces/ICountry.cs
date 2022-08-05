using System.Collections.Generic;

namespace AM.Services.Common.Contracts.Models.Entity.Interfaces;

public interface ICountry<TAsset> : ICatalog where TAsset : class, IAsset
{
    IEnumerable<TAsset>? Assets { get; set; }
}