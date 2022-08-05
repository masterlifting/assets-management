using System.Collections.Generic;
using AM.Services.Recommendations.Domain.Entities;

namespace AM.Services.Recommendations.Domain.DataAccess.Comparators;

    public class PurchaseComparer : IEqualityComparer<Purchase>
{
    public bool Equals(Purchase? x, Purchase? y) => x!.Id != 0 && x.Id == y!.Id;
    public int GetHashCode(Purchase obj) => obj.Id.GetHashCode();
}