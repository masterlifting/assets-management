using System.Linq.Expressions;

using Shared.Persistense.Abstractions.Entities.EntityPeriod;

using static Shared.Persistense.Constants.Enums;

namespace Shared.Persistense.Filters.EntityPeriod;

public sealed class EntityQuarterFilter<T> : IEntityFilter<T> where T : class, IEntityQuarter
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