using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.PeriodsOfEntity;

using System.Linq.Expressions;

using static Shared.Persistense.Constants.Enums;

namespace Shared.Persistense.QueryFilters.PeriodsOfEntity;

public sealed class EntityDateFilter<T> : IEntityPeriodFilter<T> where T : class, IEntityDate, IEntity
{
    public int Year { get; }
    public int Month { get; }
    public int Day { get; }

    public Expression<Func<T, bool>> Predicate { get; set; }

    public EntityDateFilter(Comparisons comparisons, int year)
    {
        Year = year;
        Month = 1;
        Day = 1;

        Predicate = comparisons switch
        {
            Comparisons.Equal => x => x.Date.Year == Year,
            Comparisons.More => x => x.Date.Year >= Year,
            Comparisons.Less => x => x.Date.Year <= Year,
            _ => throw new ArgumentOutOfRangeException(nameof(comparisons), comparisons, null)
        };
    }
    public EntityDateFilter(Comparisons comparisons, int year, int month)
    {
        Year = year;
        Month = month;
        Day = 1;

        Predicate = comparisons switch
        {
            Comparisons.Equal => x => x.Date.Year == Year && x.Date.Month == Month,
            Comparisons.More => x => x.Date.Year > Year || x.Date.Year == Year && x.Date.Month >= Month,
            Comparisons.Less => x => x.Date.Year < Year || x.Date.Year == Year && x.Date.Month <= Month,
            _ => throw new ArgumentOutOfRangeException(nameof(comparisons), comparisons, null)
        };
    }
    public EntityDateFilter(Comparisons comparisons, int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;

        Predicate = comparisons switch
        {
            Comparisons.Equal => x => x.Date.Year == Year && x.Date.Month == Month && x.Date.Day == Day,
            Comparisons.More => x => x.Date.Year > Year || x.Date.Year == Year && (x.Date.Month == Month && x.Date.Day >= Day || x.Date.Month > Month),
            Comparisons.Less => x => x.Date.Year < Year || x.Date.Year == Year && (x.Date.Month == Month && x.Date.Day <= Day || x.Date.Month < Month),
            _ => throw new ArgumentOutOfRangeException(nameof(comparisons), comparisons, null)
        };
    }
}