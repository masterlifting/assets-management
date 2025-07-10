using System.Collections.Generic;
using AM.Services.Recommendations.Domain.Entities;

namespace AM.Services.Recommendations.Domain.DataAccess.Comparators;

    public class SaleComparer : IEqualityComparer<Sale>
{
    public bool Equals(Sale? x, Sale? y) => x!.Id != 0 && x.Id == y!.Id;
    public int GetHashCode(Sale obj) => obj.Id.GetHashCode();
}