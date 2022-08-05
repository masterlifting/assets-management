using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using AM.Services.Market.Domain.Entities.Interfaces;

namespace AM.Services.Market.Domain.DataAccess.Comparators;

public class DataDateComparer<T> : IEqualityComparer<T> where T : class, IDataIdentity, IDateIdentity
{
    public bool Equals(T? x, T? y) => (x!.CompanyId, x.SourceId, x.Date) == (y!.CompanyId, y.SourceId, y.Date);
    public int GetHashCode(T obj) => (obj.CompanyId, obj.SourceId, obj.Date).GetHashCode();
}