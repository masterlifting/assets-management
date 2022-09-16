using Shared.Infrastructure.Persistense.Entities.EntityPeriod;
using Shared.Infrastructure.Persistense.Enums;

using System.Linq.Expressions;

namespace Shared.Infrastructure.Persistense.Filters;

public class EntityQuarterFilter<T> : IEntityFilter<T> where T : class, IEntityQuarter
{
    public int Year { get; }
    public byte Quarter { get; }
    public Expression<Func<T, bool>> Predicate { get; set; }

    public EntityQuarterFilter(Comparisons comparisons, int year)
    {
        Year = year;
        Quarter = 1;
        
        Predicate = comparisons switch
        {
            Comparisons.Equal => x => x.Year == Year,
            Comparisons.More => x => x.Year >= Year,
            Comparisons.Less => x => x.Year <= Year,
            _ => throw new ArgumentOutOfRangeException(nameof(comparisons), comparisons, null)
        };
    }
    public EntityQuarterFilter(Comparisons comparisons, int year, byte quarter)
    {
        Year = year;
        Quarter = quarter;
        
        Predicate = comparisons switch
        {
            Comparisons.Equal => x => x.Year == Year && x.Quarter == Quarter,
            Comparisons.More => x => x.Year > Year || x.Year == Year && x.Quarter >= Quarter,
            Comparisons.Less => x => x.Year < Year || x.Year == Year && x.Quarter <= Quarter,
            _ => throw new ArgumentOutOfRangeException(nameof(comparisons), comparisons, null)
        };
    }
}