using System.Collections.Generic;
using AM.Services.Portfolio.Domain.Entities;

namespace AM.Services.Portfolio.Domain.DataAccess.Comparators;

public class AssetComparer : IEqualityComparer<Asset>
{
    public bool Equals(Asset? x, Asset? y) => (x!.Id, x.TypeId) == (y!.Id, y.TypeId);
    public int GetHashCode(Asset obj) => (obj.Id, obj.TypeId).GetHashCode();
}