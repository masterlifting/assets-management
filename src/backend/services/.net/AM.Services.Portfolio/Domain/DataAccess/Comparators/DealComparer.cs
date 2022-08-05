using System.Collections.Generic;
using AM.Services.Portfolio.Domain.Entities;

namespace AM.Services.Portfolio.Domain.DataAccess.Comparators;

    public class DealComparer : IEqualityComparer<Deal>
{
    public bool Equals(Deal? x, Deal? y) => x?.Id == y!.Id;
    public int GetHashCode(Deal obj) => obj.Id.GetHashCode();
}