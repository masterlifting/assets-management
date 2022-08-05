using AM.Services.Market.Domain.Entities;

namespace AM.Services.Market.Domain.DataAccess.Comparators;

public class RatingComparer : IEqualityComparer<Rating>
{
    public bool Equals(Rating? x, Rating? y) => x!.CompanyId == y!.CompanyId;
    public int GetHashCode(Rating obj) => obj.CompanyId.GetHashCode();
}